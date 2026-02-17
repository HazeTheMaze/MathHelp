namespace MathHelpApp.Models;

public record MultiplicationProblem(int Multiplicand, int Multiplier)
{
    public int Answer => Multiplicand * Multiplier;
    public string Question => $"{Multiplicand} x {Multiplier} = ";
}
