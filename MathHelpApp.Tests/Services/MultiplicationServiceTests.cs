using MathHelpApp.Constants;
using MathHelpApp.Services;
using Shouldly;

namespace MathHelpApp.Tests.Services;

public sealed class MultiplicationServiceTests
{
    private MultiplicationService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new MultiplicationService();
    }

    [Test]
    public void GenerateTableProblems_ShouldReturn12Problems()
    {
        var problems = _service.GenerateTableProblems(5);

        problems.Count.ShouldBe(MathConstants.MaxTableNumber);
    }

    [Test]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(12)]
    public void GenerateTableProblems_AllProblemsShouldHaveCorrectMultiplicand(int tableNumber)
    {
        var problems = _service.GenerateTableProblems(tableNumber);

        problems.ShouldAllBe(p => p.Multiplicand == tableNumber);
    }

    [Test]
    public void GenerateTableProblems_MultipliersShouldRangeFrom1To12()
    {
        var problems = _service.GenerateTableProblems(7);

        var multipliers = problems.Select(p => p.Multiplier).ToList();
        var expected = Enumerable.Range(MathConstants.MinTableNumber, MathConstants.MaxTableNumber).ToList();

        multipliers.ShouldBe(expected);
    }

    [Test]
    public void GenerateTableProblems_ShouldCalculateCorrectAnswers()
    {
        var problems = _service.GenerateTableProblems(6);

        problems.ShouldAllBe(p => p.Answer == p.Multiplicand * p.Multiplier);
    }

    [Test]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    public void GenerateRandomProblems_ShouldReturnRequestedCount(int count)
    {
        var problems = _service.GenerateRandomProblems(1, 12, count);

        problems.Count.ShouldBe(count);
    }

    [Test]
    public void GenerateRandomProblems_ShouldGenerateProblemsWithinTableRange()
    {
        const int minTable = 3;
        const int maxTable = 7;

        var problems = _service.GenerateRandomProblems(minTable, maxTable, 100);

        problems.ShouldAllBe(p =>
            p.Multiplicand >= minTable && p.Multiplicand <= maxTable &&
            p.Multiplier >= MathConstants.MinTableNumber && p.Multiplier <= maxTable);
    }

    [Test]
    public void GenerateRandomProblems_ShouldGenerateVariedProblems()
    {
        var problems = _service.GenerateRandomProblems(1, 12, 50);

        // With 50 random problems from tables 1-12, we should have variety
        var distinctCount = problems.Distinct().Count();
        distinctCount.ShouldBeGreaterThan(1);
    }

    [Test]
    public void GenerateRandomProblems_WithSingleTable_ShouldOnlyUseThatTable()
    {
        const int tableNumber = 5;

        var problems = _service.GenerateRandomProblems(tableNumber, tableNumber, 20);

        problems.ShouldAllBe(p => p.Multiplicand == tableNumber);
    }
}
