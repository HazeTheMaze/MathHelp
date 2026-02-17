using MathHelpApp.Models;

namespace MathHelpApp.Services;

public interface IMultiplicationService
{
    /// <summary>
    /// Generates all problems for a single multiplication table (1-12).
    /// </summary>
    /// <param name="tableNumber">The multiplication table number.</param>
    /// <returns>A list of multiplication problems for the specified table.</returns>
    List<MultiplicationProblem> GenerateTableProblems(int tableNumber);

    /// <summary>
    /// Generates random multiplication problems within the specified table range.
    /// </summary>
    List<MultiplicationProblem> GenerateRandomProblems(int minTable, int maxTable, int count);
}
