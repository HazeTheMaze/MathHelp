using MathHelpApp.Constants;
using Shouldly;

namespace MathHelpApp.Tests.Constants;

public sealed class MathConstantsTests
{
    [Test]
    public void MinTableNumber_ShouldBe1()
    {
        MathConstants.MinTableNumber.ShouldBe(1);
    }

    [Test]
    public void MaxTableNumber_ShouldBe12()
    {
        MathConstants.MaxTableNumber.ShouldBe(12);
    }

    [Test]
    public void ProblemsPerSheet_ShouldBe100()
    {
        MathConstants.ProblemsPerSheet.ShouldBe(100);
    }

    [Test]
    public void ProblemsPerGroup_ShouldBe10()
    {
        MathConstants.ProblemsPerGroup.ShouldBe(10);
    }

    [Test]
    public void GroupsPerSheet_ShouldEqualProblemsPerSheetDividedByProblemsPerGroup()
    {
        var expected = MathConstants.ProblemsPerSheet / MathConstants.ProblemsPerGroup;

        MathConstants.GroupsPerSheet.ShouldBe(expected);
    }

    [Test]
    public void GroupsPerSheet_ShouldBe10()
    {
        // Verify the actual calculated value
        MathConstants.GroupsPerSheet.ShouldBe(10);
    }

    [Test]
    public void ColumnsPerRow_ShouldBe2()
    {
        MathConstants.ColumnsPerRow.ShouldBe(2);
    }

    [Test]
    public void MaxSheetCount_ShouldBe100()
    {
        MathConstants.MaxSheetCount.ShouldBe(100);
    }

    [Test]
    public void ProblemsPerSheet_ShouldBeDivisibleByProblemsPerGroup()
    {
        // This ensures GroupsPerSheet calculation doesn't lose precision
        (MathConstants.ProblemsPerSheet % MathConstants.ProblemsPerGroup).ShouldBe(0);
    }

    [Test]
    public void GroupsPerSheet_ShouldBeDivisibleByColumnsPerRow_ForEvenLayout()
    {
        // 2×5 layout: 10 groups / 2 columns = 5 full rows, no orphan groups
        var remainder = MathConstants.GroupsPerSheet % MathConstants.ColumnsPerRow;
        remainder.ShouldBe(0);
    }
}
