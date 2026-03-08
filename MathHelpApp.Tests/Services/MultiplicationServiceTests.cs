using MathHelpApp.Constants;
using MathHelpApp.Models;
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
    public void GenerateRandomProblems_WithCountZero_ShouldReturnEmptyList()
    {
        var problems = _service.GenerateRandomProblems(1, 12, 0);

        problems.ShouldBeEmpty();
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

    [Test]
    public void GenerateRandomProblems_WithMinGreaterThanMax_ShouldThrow()
    {
        Should.Throw<ArgumentOutOfRangeException>(
            () => _service.GenerateRandomProblems(10, 5, 10));
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolIsLargeEnough_ShouldReturnAllUnique()
    {
        var problems = _service.GenerateRandomProblems(1, 12, 100);

        var distinctCount = problems.Distinct().Count();
        distinctCount.ShouldBe(100);
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolEqualsCount_ShouldReturnAllUnique()
    {
        var problems = _service.GenerateRandomProblems(1, 10, 100);

        var distinctCount = problems.Distinct().Count();
        distinctCount.ShouldBe(100);
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolSmallerThanCount_ShouldDistributeEvenly()
    {
        // Range 6-10: 5 multiplicands * 10 multipliers = 50 unique, 100 / 50 = exactly 2 each
        var problems = _service.GenerateRandomProblems(6, 10, 100);

        var usageCounts = problems.GroupBy(p => p).Select(g => g.Count()).ToList();
        usageCounts.Count.ShouldBe(50);
        usageCounts.ShouldAllBe(c => c == 2);
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolAtLeast10_GroupsShouldHaveNoDuplicates()
    {
        var problems = _service.GenerateRandomProblems(5, 10, 100);

        for (int g = 0; g < problems.Count / MathConstants.ProblemsPerGroup; g++)
        {
            var group = problems
                .Skip(g * MathConstants.ProblemsPerGroup)
                .Take(MathConstants.ProblemsPerGroup)
                .ToList();

            group.Distinct().Count().ShouldBe(group.Count,
                $"Group {g} contains duplicate problems");
        }
    }

    [Test]
    public void GenerateRandomProblems_SmallPool_ShouldProduceValidProblems()
    {
        var problems = _service.GenerateRandomProblems(2, 2, 20);

        problems.Count.ShouldBe(20);
        problems.ShouldAllBe(p => p.Multiplicand == 2 && p.Multiplier >= 1 && p.Multiplier <= 2);
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolLargerThanCount_AllReturnedShouldBeUnique()
    {
        // Range 1-12: 12 * 12 = 144 unique, requesting only 100
        var problems = _service.GenerateRandomProblems(1, 12, 100);

        problems.Distinct().Count().ShouldBe(100);
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolSmallerThanCount_FirstPoolSizeProblemsAreAllUnique()
    {
        // Range 6-10: 5 multiplicands * 10 multipliers = 50 unique, requesting 100
        const int expectedPoolSize = 50;
        var problems = _service.GenerateRandomProblems(6, 10, 100);

        var firstPool = problems.Take(expectedPoolSize).ToList();
        firstPool.Distinct().Count().ShouldBe(expectedPoolSize,
            "The first complete deal of the pool should contain every unique problem exactly once");
    }

    [Test]
    public void GenerateRandomProblems_WhenPoolNotMultipleOf10_GroupsShouldHaveNoDuplicates()
    {
        // Range 3-5: 3 multiplicands * 5 multipliers = 15 unique (not a multiple of 10).
        // Requesting 30 forces two reshuffles, both landing mid-group.
        var problems = _service.GenerateRandomProblems(3, 5, 30);

        for (int g = 0; g < problems.Count / MathConstants.ProblemsPerGroup; g++)
        {
            var group = problems
                .Skip(g * MathConstants.ProblemsPerGroup)
                .Take(MathConstants.ProblemsPerGroup)
                .ToList();

            group.Distinct().Count().ShouldBe(group.Count,
                $"Group {g} contains duplicate problems");
        }
    }

    // --- Shuffle ---

    [Test]
    public void Shuffle_ShouldPreserveAllElements()
    {
        var list = Enumerable.Range(1, 20).ToList();
        var sorted = new List<int>(list);

        _service.Shuffle(list);

        list.OrderBy(x => x).ShouldBe(sorted);
    }

    [Test]
    public void Shuffle_WithLargeList_ShouldChangeOrder()
    {
        var list = Enumerable.Range(1, 100).ToList();
        var original = new List<int>(list);

        _service.Shuffle(list);

        list.ShouldNotBe(original);
    }

    [Test]
    public void Shuffle_WithSingleElement_ShouldNotThrow()
    {
        var list = new List<int> { 42 };

        Should.NotThrow(() => _service.Shuffle(list));

        list.ShouldBe([42]);
    }

    [Test]
    public void Shuffle_WithEmptyList_ShouldNotThrow()
    {
        var list = new List<int>();

        Should.NotThrow(() => _service.Shuffle(list));

        list.ShouldBeEmpty();
    }

    // --- BuildPool ---

    [Test]
    [TestCase(3, 5, 15)]
    [TestCase(1, 10, 100)]
    [TestCase(6, 10, 50)]
    public void BuildPool_ShouldReturnCorrectCount(int min, int max, int expected)
    {
        var pool = MultiplicationService.BuildPool(min, max);

        pool.Count.ShouldBe(expected);
    }

    [Test]
    public void BuildPool_ShouldContainAllCombinations()
    {
        var pool = MultiplicationService.BuildPool(2, 3);

        pool.Count.ShouldBe(6);
        pool.ShouldContain(new MultiplicationProblem(2, 1));
        pool.ShouldContain(new MultiplicationProblem(2, 2));
        pool.ShouldContain(new MultiplicationProblem(2, 3));
        pool.ShouldContain(new MultiplicationProblem(3, 1));
        pool.ShouldContain(new MultiplicationProblem(3, 2));
        pool.ShouldContain(new MultiplicationProblem(3, 3));
    }

    // --- FindNonConflicting ---

    [Test]
    public void FindNonConflicting_ShouldReturnFirstNonConflictingIndex()
    {
        var deck = new List<MultiplicationProblem>
        {
            new(1, 1), new(1, 2), new(1, 3), new(1, 4)
        };
        var used = new HashSet<MultiplicationProblem> { new(1, 1), new(1, 2), new(1, 3) };

        var idx = MultiplicationService.FindNonConflicting(deck, used, 1);

        idx.ShouldBe(3);
    }

    [Test]
    public void FindNonConflicting_WhenAllConflict_ShouldReturnNegativeOne()
    {
        var deck = new List<MultiplicationProblem> { new(1, 1), new(1, 2) };
        var used = new HashSet<MultiplicationProblem> { new(1, 1), new(1, 2) };

        var idx = MultiplicationService.FindNonConflicting(deck, used, 0);

        idx.ShouldBe(-1);
    }

    [Test]
    public void FindNonConflicting_ShouldSearchFromSpecifiedIndex()
    {
        var deck = new List<MultiplicationProblem> { new(1, 1), new(1, 2), new(1, 3) };
        var used = new HashSet<MultiplicationProblem> { new(1, 2) };

        // 1x1 at index 0 is non-conflicting but searchFrom=1 skips it
        var idx = MultiplicationService.FindNonConflicting(deck, used, 1);

        idx.ShouldBe(2);
    }

    // --- ReshuffleAvoidingGroupConflicts ---

    [Test]
    public void ReshuffleAvoidingGroupConflicts_AtGroupBoundary_ShouldPreserveAllDeckElements()
    {
        var pool = MultiplicationService.BuildPool(1, 5);
        var deck = new List<MultiplicationProblem>(pool);
        var result = deck.Take(20).ToList();
        var expectedElements = new HashSet<MultiplicationProblem>(deck);

        _service.ReshuffleAvoidingGroupConflicts(deck, result);

        deck.Count.ShouldBe(expectedElements.Count);
        new HashSet<MultiplicationProblem>(deck).SetEquals(expectedElements).ShouldBeTrue();
    }

    [Test]
    public void ReshuffleAvoidingGroupConflicts_MidGroup_ShouldNotConflictWithCurrentGroup()
    {
        // Pool of 15 (range 3-5: 3 multiplicands * 5 multipliers)
        var pool = MultiplicationService.BuildPool(3, 5);
        var deck = new List<MultiplicationProblem>(pool);

        // 13 items dealt: group 1 spans positions 10-19, positions 10-12 already filled
        var result = pool.Take(13).ToList();
        var alreadyInGroup = new HashSet<MultiplicationProblem>(result.Skip(10));

        _service.ReshuffleAvoidingGroupConflicts(deck, result);

        int slotsToFill = MathConstants.ProblemsPerGroup - alreadyInGroup.Count;
        var deckHead = deck.Take(slotsToFill).ToList();
        foreach (var problem in deckHead)
        {
            alreadyInGroup.ShouldNotContain(problem,
                $"{problem.Question} conflicts with an item already in the current group");
        }
    }
}
