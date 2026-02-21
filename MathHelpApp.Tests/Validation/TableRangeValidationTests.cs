using MathHelpApp.Constants;
using MathHelpApp.Validation;
using Shouldly;

namespace MathHelpApp.Tests.Validation;

public sealed class TableRangeValidationTests
{
    [Test]
    [TestCase(1, 12)]
    [TestCase(1, 1)]
    [TestCase(12, 12)]
    [TestCase(5, 10)]
    public void Validate_WithValidRange_ShouldReturnNull(int minTable, int maxTable)
    {
        var result = TableRangeValidation.Validate(minTable, maxTable);

        result.ShouldBeNull();
    }

    [Test]
    [TestCase(0, 12)]
    [TestCase(1, 13)]
    [TestCase(-1, 5)]
    [TestCase(5, 100)]
    public void Validate_WithOutOfRangeValues_ShouldReturnErrorMessage(int minTable, int maxTable)
    {
        var result = TableRangeValidation.Validate(minTable, maxTable);

        result.ShouldNotBeNull();
    }

    [Test]
    public void Validate_WithMinGreaterThanMax_ShouldReturnExactMessage()
    {
        var result = TableRangeValidation.Validate(10, 5);

        result.ShouldNotBeNull();
        result.ShouldBe("Validation.MinGreaterThanMax");
    }

    [Test]
    public void Validate_WithMinTableOutOfRange_ShouldReturnExactMessage()
    {
        var result = TableRangeValidation.Validate(0, 5);

        result.ShouldBe("Validation.MinTableRange");
    }

    [Test]
    public void Validate_WithMaxTableOutOfRange_ShouldReturnExactMessage()
    {
        var result = TableRangeValidation.Validate(1, 13);

        result.ShouldBe("Validation.MaxTableRange");
    }

    [Test]
    public void Validate_WithOutOfRange_ShouldReturnRangeKey()
    {
        var result = TableRangeValidation.Validate(0, 5);

        result.ShouldNotBeNull();
        result.ShouldBeOneOf("Validation.MinTableRange", "Validation.MaxTableRange");
    }

    [Test]
    public void Validate_AtBoundaries_ShouldBeValid()
    {
        var minBoundary = TableRangeValidation.Validate(
            MathConstants.MinTableNumber, MathConstants.MinTableNumber);
        var maxBoundary = TableRangeValidation.Validate(
            MathConstants.MaxTableNumber, MathConstants.MaxTableNumber);
        var fullRange = TableRangeValidation.Validate(
            MathConstants.MinTableNumber, MathConstants.MaxTableNumber);

        minBoundary.ShouldBeNull();
        maxBoundary.ShouldBeNull();
        fullRange.ShouldBeNull();
    }

    [Test]
    public void Validate_JustOutsideBoundaries_ShouldBeInvalid()
    {
        var belowMin = TableRangeValidation.Validate(
            MathConstants.MinTableNumber - 1, MathConstants.MaxTableNumber);
        var aboveMax = TableRangeValidation.Validate(
            MathConstants.MinTableNumber, MathConstants.MaxTableNumber + 1);

        belowMin.ShouldNotBeNull();
        aboveMax.ShouldNotBeNull();
    }
}
