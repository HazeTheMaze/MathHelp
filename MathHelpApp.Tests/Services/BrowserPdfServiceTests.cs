using System.Text.Json;
using MathHelpApp.Models;
using MathHelpApp.Resources;
using MathHelpApp.Services;
using MathHelpApp.Tests.Fakes;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Shouldly;

namespace MathHelpApp.Tests.Services;

public sealed class BrowserPdfServiceTests
{
    private readonly IStringLocalizer<SharedResources> _loc = Substitute.For<IStringLocalizer<SharedResources>>();

    private static void SetupLocalizer(IStringLocalizer<SharedResources> loc)
    {
        loc["PdfTitleMultiplicationTables"].Returns(new LocalizedString("PdfTitleMultiplicationTables", "Multiplication Tables"));
        loc["PdfTitlePageSuffix"].Returns(new LocalizedString("PdfTitlePageSuffix", " (Page {0})"));
        loc["PdfPracticeSheet"].Returns(new LocalizedString("PdfPracticeSheet", "Practice"));
        loc["PdfAnswerSheet"].Returns(new LocalizedString("PdfAnswerSheet", "Answers"));
        loc["PdfSheetNumberSuffix"].Returns(new LocalizedString("PdfSheetNumberSuffix", " #{0}"));
    }

    [Test]
    public async Task DownloadTablePdfAsync_ShouldInvokeJsWithTableTypeAndCorrectPayload()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://example.com/");
        var sut = new BrowserPdfService(js, navigation, _loc);

        await sut.DownloadTablePdfAsync(2, 4, showAnswers: true);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        var root = doc.RootElement;
        root.GetProperty("type").GetString().ShouldBe("table");
        root.GetProperty("minTable").GetInt32().ShouldBe(2);
        root.GetProperty("maxTable").GetInt32().ShouldBe(4);
        root.GetProperty("showAnswers").GetBoolean().ShouldBe(true);
        root.GetProperty("problemsPerGroup").GetInt32().ShouldBe(5);
        root.GetProperty("siteName").GetString().ShouldBe("MathHelp");
        root.GetProperty("siteUrl").GetString().ShouldBe("https://example.com");
    }

    [Test]
    public async Task DownloadTablePdfAsync_WhenMinTableIs1_ShouldSetProblemsPerGroupToMaxTable()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://app.test/");
        var sut = new BrowserPdfService(js, navigation, _loc);

        await sut.DownloadTablePdfAsync(1, 12, showAnswers: false);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        doc.RootElement.GetProperty("problemsPerGroup").GetInt32().ShouldBe(12);
    }

    [Test]
    public async Task DownloadTablePdfAsync_ShouldBuildProblemsForEachTableAndMultiplier()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://example.com/");
        var sut = new BrowserPdfService(js, navigation, _loc);

        await sut.DownloadTablePdfAsync(3, 4, showAnswers: true);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        var problems = doc.RootElement.GetProperty("problems");
        problems.GetArrayLength().ShouldBe(2 * 5); // tables 3,4 × 5 problems each (problemsPerGroup = 5 when minTable != 1)
        var first = problems[0];
        first.GetProperty("a").GetInt32().ShouldBe(3);
        first.GetProperty("b").GetInt32().ShouldBe(1);
        first.GetProperty("ans").GetInt32().ShouldBe(3);
    }

    [Test]
    public async Task DownloadTablePdfAsync_ShouldSetSiteUrlWithoutTrailingSlash()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://mysite.com/subpath/");
        var sut = new BrowserPdfService(js, navigation, _loc);

        await sut.DownloadTablePdfAsync(1, 1, showAnswers: false);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        doc.RootElement.GetProperty("siteUrl").GetString().ShouldBe("https://mysite.com/subpath");
    }

    [Test]
    public async Task DownloadTablePdfAsync_WhenBaseUriIsRoot_ShouldSetSiteUrlToAuthorityOnly()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://root.com/");
        var sut = new BrowserPdfService(js, navigation, _loc);

        await sut.DownloadTablePdfAsync(1, 1, showAnswers: false);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        doc.RootElement.GetProperty("siteUrl").GetString().ShouldBe("https://root.com");
    }

    [Test]
    public async Task DownloadPracticePdfAsync_ShouldInvokeJsWithPracticeTypeAndSheets()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://example.com/");
        var sut = new BrowserPdfService(js, navigation, _loc);
        var sheet = new List<MultiplicationProblem> { new(2, 3), new(4, 5) };
        var allSheets = new List<List<MultiplicationProblem>> { sheet };

        await sut.DownloadPracticePdfAsync(allSheets);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        var root = doc.RootElement;
        root.GetProperty("type").GetString().ShouldBe("practice");
        root.TryGetProperty("showAnswers", out _).ShouldBeFalse();
        var sheets = root.GetProperty("sheets");
        sheets.GetArrayLength().ShouldBe(1);
        var firstSheet = sheets[0];
        firstSheet.GetArrayLength().ShouldBe(2);
        firstSheet[0].GetProperty("a").GetInt32().ShouldBe(2);
        firstSheet[0].GetProperty("b").GetInt32().ShouldBe(3);
        firstSheet[0].GetProperty("ans").GetInt32().ShouldBe(6);
        firstSheet[1].GetProperty("a").GetInt32().ShouldBe(4);
        firstSheet[1].GetProperty("b").GetInt32().ShouldBe(5);
        firstSheet[1].GetProperty("ans").GetInt32().ShouldBe(20);
        root.GetProperty("siteUrl").GetString().ShouldBe("https://example.com");
    }

    [Test]
    public async Task DownloadPracticeAnswerSheetPdfAsync_ShouldSetShowAnswersTrue()
    {
        SetupLocalizer(_loc);
        var js = new FakeJSRuntime();
        var navigation = new FakeNavigationManager("https://example.com/");
        var sut = new BrowserPdfService(js, navigation, _loc);
        var allSheets = new List<List<MultiplicationProblem>> { new List<MultiplicationProblem> { new(1, 1) } };

        await sut.DownloadPracticeAnswerSheetPdfAsync(allSheets);

        js.LastInvokeJson.ShouldNotBeNull();
        using var doc = JsonDocument.Parse(js.LastInvokeJson!);
        doc.RootElement.GetProperty("type").GetString().ShouldBe("practice");
        doc.RootElement.GetProperty("showAnswers").GetBoolean().ShouldBe(true);
    }
}
