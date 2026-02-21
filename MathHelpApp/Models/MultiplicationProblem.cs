namespace MathHelpApp.Models;

/// <summary>
/// A single multiplication problem (e.g. 3 × 4 = 12).
/// </summary>
/// <param name="Multiplicand">First factor.</param>
/// <param name="Multiplier">Second factor.</param>
public sealed record MultiplicationProblem(int Multiplicand, int Multiplier)
{
    /// <summary>Product of <see cref="Multiplicand"/> and <see cref="Multiplier"/>.</summary>
    public int Answer => Multiplicand * Multiplier;

    /// <summary>Human-readable question string (e.g. "3 x 4 = ").</summary>
    public string Question => $"{Multiplicand} x {Multiplier} = ";
}
