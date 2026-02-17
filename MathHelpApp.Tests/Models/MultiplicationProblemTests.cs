using MathHelpApp.Models;
using Shouldly;

namespace MathHelpApp.Tests.Models;

public sealed class MultiplicationProblemTests
{
    [Test]
    [TestCase(3, 4, 12)]
    [TestCase(7, 8, 56)]
    [TestCase(12, 12, 144)]
    [TestCase(1, 1, 1)]
    [TestCase(0, 5, 0)]
    public void Answer_ShouldReturnCorrectProduct(int multiplicand, int multiplier, int expectedAnswer)
    {
        var problem = new MultiplicationProblem(multiplicand, multiplier);

        problem.Answer.ShouldBe(expectedAnswer);
    }

    [Test]
    [TestCase(3, 4, "3 x 4 = ")]
    [TestCase(12, 11, "12 x 11 = ")]
    [TestCase(1, 1, "1 x 1 = ")]
    public void Question_ShouldReturnCorrectFormat(int multiplicand, int multiplier, string expectedQuestion)
    {
        var problem = new MultiplicationProblem(multiplicand, multiplier);

        problem.Question.ShouldBe(expectedQuestion);
    }

    [Test]
    public void Record_ShouldSupportValueEquality()
    {
        var problem1 = new MultiplicationProblem(5, 6);
        var problem2 = new MultiplicationProblem(5, 6);

        problem1.ShouldBe(problem2);
    }

    [Test]
    public void Record_ShouldNotBeEqualWithDifferentValues()
    {
        var problem1 = new MultiplicationProblem(5, 6);
        var problem2 = new MultiplicationProblem(6, 5);

        problem1.ShouldNotBe(problem2);
    }
}
