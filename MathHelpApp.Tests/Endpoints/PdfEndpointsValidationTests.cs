using MathHelpApp.Constants;
using MathHelpApp.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;

namespace MathHelpApp.Tests.Endpoints;

public sealed class PdfEndpointsValidationTests
{
    [Test]
    [TestCase(1, 12)]
    [TestCase(1, 1)]
    [TestCase(12, 12)]
    [TestCase(5, 10)]
    public void ValidateTableRange_WithValidRange_ShouldReturnNull(int minTable, int maxTable)
    {
        var result = PdfEndpoints.ValidateTableRange(minTable, maxTable);

        result.ShouldBeNull();
    }

    [Test]
    [TestCase(0, 12)]
    [TestCase(1, 13)]
    [TestCase(-1, 5)]
    [TestCase(5, 100)]
    public void ValidateTableRange_WithOutOfRangeValues_ShouldReturnBadRequest(int minTable, int maxTable)
    {
        var result = PdfEndpoints.ValidateTableRange(minTable, maxTable);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<BadRequest<string>>();
    }

    [Test]
    public void ValidateTableRange_WithMinGreaterThanMax_ShouldReturnBadRequest()
    {
        var result = PdfEndpoints.ValidateTableRange(10, 5);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<BadRequest<string>>();
    }

    [Test]
    public void ValidateTableRange_WithMinGreaterThanMax_ShouldContainDescriptiveMessage()
    {
        var result = PdfEndpoints.ValidateTableRange(10, 5) as BadRequest<string>;

        result.ShouldNotBeNull();
        var message = result!.Value!;
        message.ShouldContain("minTable");
        message.ShouldContain("maxTable");
    }

    [Test]
    public void ValidateTableRange_WithOutOfRange_ShouldContainRangeLimits()
    {
        var result = PdfEndpoints.ValidateTableRange(0, 5) as BadRequest<string>;

        result.ShouldNotBeNull();
        var message = result!.Value!;
        message.ShouldContain(MathConstants.MinTableNumber.ToString());
        message.ShouldContain(MathConstants.MaxTableNumber.ToString());
    }

    [Test]
    public void ValidateTableRange_AtBoundaries_ShouldBeValid()
    {
        // Test exact boundary values
        var minBoundary = PdfEndpoints.ValidateTableRange(
            MathConstants.MinTableNumber, MathConstants.MinTableNumber);
        var maxBoundary = PdfEndpoints.ValidateTableRange(
            MathConstants.MaxTableNumber, MathConstants.MaxTableNumber);
        var fullRange = PdfEndpoints.ValidateTableRange(
            MathConstants.MinTableNumber, MathConstants.MaxTableNumber);

        minBoundary.ShouldBeNull();
        maxBoundary.ShouldBeNull();
        fullRange.ShouldBeNull();
    }

    [Test]
    public void ValidateTableRange_JustOutsideBoundaries_ShouldBeInvalid()
    {
        var belowMin = PdfEndpoints.ValidateTableRange(
            MathConstants.MinTableNumber - 1, MathConstants.MaxTableNumber);
        var aboveMax = PdfEndpoints.ValidateTableRange(
            MathConstants.MinTableNumber, MathConstants.MaxTableNumber + 1);

        belowMin.ShouldNotBeNull();
        aboveMax.ShouldNotBeNull();
    }
}
