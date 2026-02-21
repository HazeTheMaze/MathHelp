#pragma warning disable IDE0005 // Using directive is unnecessary (analyser reports NSubstitute as unused)
using MathHelpApp.Components.Pages;
using MathHelpApp.Models;
using MathHelpApp.Resources;
using MathHelpApp.Services;
using MathHelpApp.Tests.Fakes;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Shouldly;
#pragma warning restore IDE0005

namespace MathHelpApp.Tests.Components.Pages;

public sealed class HomePageTests
{
    [Test]
    public async Task OnDownloadPdf_WhenUriHasTabPractice_ShouldCallPracticePdfServices()
    {
        var nav = new FakeNavigationManager("https://example.com/", "https://example.com/?tab=practice");
        var pdfService = Substitute.For<IBrowserPdfService>();
        var mathService = Substitute.For<IMultiplicationService>();
        mathService
            .GenerateRandomProblems(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(new List<MultiplicationProblem> { new(2, 3) });
        var loc = Substitute.For<IStringLocalizer<SharedResources>>();
        loc["DownloadFailed"].Returns(new LocalizedString("DownloadFailed", "Failed"));
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<Home>>();

        var home = new Home
        {
            Navigation = nav,
            PdfService = pdfService,
            MathService = mathService,
            Loc = loc,
            Logger = logger,
        };
        home.InitializeTabFromQuery();
        await home.OnDownloadPdf();

        await pdfService.Received(1).DownloadPracticePdfAsync(Arg.Any<List<List<MultiplicationProblem>>>());
        await pdfService.Received(1).DownloadPracticeAnswerSheetPdfAsync(Arg.Any<List<List<MultiplicationProblem>>>());
        await pdfService.DidNotReceive().DownloadTablePdfAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task OnDownloadPdf_WhenUriHasNoTabOrTabReference_ShouldCallTablePdfService()
    {
        var nav = new FakeNavigationManager("https://example.com/", "https://example.com/");
        var pdfService = Substitute.For<IBrowserPdfService>();
        var mathService = Substitute.For<IMultiplicationService>();
        var loc = Substitute.For<IStringLocalizer<SharedResources>>();
        loc["DownloadFailed"].Returns(new LocalizedString("DownloadFailed", "Failed"));
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<Home>>();

        var home = new Home
        {
            Navigation = nav,
            PdfService = pdfService,
            MathService = mathService,
            Loc = loc,
            Logger = logger,
        };
        home.InitializeTabFromQuery();
        await home.OnDownloadPdf();

        await pdfService.Received(1).DownloadTablePdfAsync(1, 12, true);
        await pdfService.DidNotReceive().DownloadPracticePdfAsync(Arg.Any<List<List<MultiplicationProblem>>>());
        await pdfService.DidNotReceive().DownloadPracticeAnswerSheetPdfAsync(Arg.Any<List<List<MultiplicationProblem>>>());
    }

    [Test]
    public async Task OnDownloadPdf_WhenUriHasTabReference_ShouldCallTablePdfService()
    {
        var nav = new FakeNavigationManager("https://example.com/", "https://example.com/?tab=reference");
        var pdfService = Substitute.For<IBrowserPdfService>();
        var mathService = Substitute.For<IMultiplicationService>();
        var loc = Substitute.For<IStringLocalizer<SharedResources>>();
        loc["DownloadFailed"].Returns(new LocalizedString("DownloadFailed", "Failed"));
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<Home>>();

        var home = new Home
        {
            Navigation = nav,
            PdfService = pdfService,
            MathService = mathService,
            Loc = loc,
            Logger = logger,
        };
        home.InitializeTabFromQuery();
        await home.OnDownloadPdf();

        await pdfService.Received(1).DownloadTablePdfAsync(1, 12, true);
        await pdfService.DidNotReceive().DownloadPracticePdfAsync(Arg.Any<List<List<MultiplicationProblem>>>());
    }
}
