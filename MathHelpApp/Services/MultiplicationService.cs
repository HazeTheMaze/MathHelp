using MathHelpApp.Constants;
using MathHelpApp.Models;

namespace MathHelpApp.Services;

public sealed class MultiplicationService : IMultiplicationService
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
        var problems = new List<MultiplicationProblem>();

        for (int i = 0; i < count; i++)
        {
            int multiplicand = _random.Next(minTable, maxTable + 1);
            int multiplier = _random.Next(MathConstants.MinTableNumber, maxTable + 1);
            problems.Add(new MultiplicationProblem(multiplicand, multiplier));
        }

        return problems;
    }
}
