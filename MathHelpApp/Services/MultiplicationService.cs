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
        var usageCounts = pool.ToDictionary(p => p, _ => 0);
        var result = new List<MultiplicationProblem>(count);

        while (result.Count < count)
        {
            int needed = Math.Min(MathConstants.ProblemsPerGroup, count - result.Count);
            var group = SelectGroupProblems(pool, usageCounts, needed);
            result.AddRange(group);
        }

        return result;
    }

    private static List<MultiplicationProblem> BuildPool(int minTable, int maxTable)
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

    private List<MultiplicationProblem> SelectGroupProblems(
        List<MultiplicationProblem> pool,
        Dictionary<MultiplicationProblem, int> usageCounts,
        int needed)
    {
        var group = new List<MultiplicationProblem>(needed);
        var usedInGroup = new HashSet<MultiplicationProblem>();

        for (int i = 0; i < needed; i++)
        {
            var candidates = pool
                .Where(p => !usedInGroup.Contains(p))
                .ToList();

            if (candidates.Count == 0)
            {
                candidates = new List<MultiplicationProblem>(pool);
            }

            int minUsage = candidates.Min(p => usageCounts[p]);
            var best = candidates.Where(p => usageCounts[p] == minUsage).ToList();

            var selected = best[_random.Next(best.Count)];
            group.Add(selected);
            usedInGroup.Add(selected);
            usageCounts[selected]++;
        }

        Shuffle(group);
        return group;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
