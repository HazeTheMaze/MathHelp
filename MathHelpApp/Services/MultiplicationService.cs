using MathHelpApp.Models;

namespace MathHelpApp.Services;

public class MultiplicationService : IMultiplicationService
{
    private readonly Random _random = new();

    public List<MultiplicationProblem> GenerateTableProblems(int tableNumber)
    {
        var problems = new List<MultiplicationProblem>();
        
        for (int i = 1; i <= 12; i++)
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
            int multiplier = _random.Next(1, maxTable + 1); // Use maxTable as upper limit
            problems.Add(new MultiplicationProblem(multiplicand, multiplier));
        }
        
        return problems;
    }
}
