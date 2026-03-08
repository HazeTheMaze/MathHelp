using MathHelpApp.Constants;
using MathHelpApp.Models;

namespace MathHelpApp.Services;

internal sealed class MultiplicationService : IMultiplicationService
{
    private readonly Random _random = new();

    public List<MultiplicationProblem> GenerateTableProblems(int tableNumber)
    {
        var problems = new List<MultiplicationProblem>();

        for (int i = MathConstants.MinTableNumber; i <= MathConstants.MaxTableNumber; i++)
        {
            problems.Add(new MultiplicationProblem(tableNumber, i));
        }

        return problems;
    }

    public List<MultiplicationProblem> GenerateRandomProblems(int minTable, int maxTable, int count)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minTable, maxTable, nameof(minTable));

        if (count == 0)
        {
            return [];
        }

        var pool = BuildPool(minTable, maxTable);
        var deck = new List<MultiplicationProblem>(pool);
        Shuffle(deck);

        var result = new List<MultiplicationProblem>(count);
        int deckIndex = 0;

        while (result.Count < count)
        {
            if (deckIndex >= deck.Count)
            {
                deckIndex = 0;
                ReshuffleAvoidingGroupConflicts(deck, result);
            }

            result.Add(deck[deckIndex++]);
        }

        return result;
    }

    internal void ReshuffleAvoidingGroupConflicts(
        List<MultiplicationProblem> deck,
        List<MultiplicationProblem> result)
    {
        Shuffle(deck);

        int groupStart = (result.Count / MathConstants.ProblemsPerGroup) * MathConstants.ProblemsPerGroup;
        if (groupStart == result.Count)
        {
            return;
        }

        var usedInGroup = new HashSet<MultiplicationProblem>(result.Skip(groupStart));
        int slotsLeft = Math.Min(
            MathConstants.ProblemsPerGroup - usedInGroup.Count,
            deck.Count);

        for (int i = 0; i < slotsLeft; i++)
        {
            if (!usedInGroup.Contains(deck[i]))
            {
                usedInGroup.Add(deck[i]);
                continue;
            }

            int swapIdx = FindNonConflicting(deck, usedInGroup, slotsLeft);
            if (swapIdx >= 0)
            {
                (deck[i], deck[swapIdx]) = (deck[swapIdx], deck[i]);
            }

            usedInGroup.Add(deck[i]);
        }
    }

    internal static int FindNonConflicting(
        List<MultiplicationProblem> deck,
        HashSet<MultiplicationProblem> usedInGroup,
        int searchFrom)
    {
        for (int j = searchFrom; j < deck.Count; j++)
        {
            if (!usedInGroup.Contains(deck[j]))
            {
                return j;
            }
        }

        return -1;
    }

    internal static List<MultiplicationProblem> BuildPool(int minTable, int maxTable)
    {
        var pool = new List<MultiplicationProblem>();
        for (int a = minTable; a <= maxTable; a++)
        {
            for (int b = MathConstants.MinTableNumber; b <= maxTable; b++)
            {
                pool.Add(new MultiplicationProblem(a, b));
            }
        }

        return pool;
    }

    internal void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
