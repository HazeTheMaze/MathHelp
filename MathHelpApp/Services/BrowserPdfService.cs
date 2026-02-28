using System.Text.Json;
using MathHelpApp.Constants;
using MathHelpApp.Models;
using MathHelpApp.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace MathHelpApp.Services;

internal sealed class BrowserPdfService : IBrowserPdfService
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _navigation;
    private readonly IStringLocalizer<SharedResources> _loc;

    public BrowserPdfService(IJSRuntime js, NavigationManager navigation, IStringLocalizer<SharedResources> loc)
    {
        _js = js;
        _navigation = navigation;
        _loc = loc;
    }

    private static string GetSiteUrl(NavigationManager navigation)
    {
        var uri = new Uri(navigation.BaseUri);
        var path = uri.AbsolutePath.TrimEnd('/');
        return uri.GetLeftPart(UriPartial.Authority) + (string.IsNullOrEmpty(path) || path == "/" ? "" : path);
    }

    public async Task DownloadTablePdfAsync(int minTable, int maxTable, bool showAnswers)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minTable, maxTable, nameof(minTable));

        // Full range (1..N) shows N problems per group; partial ranges use a fixed count.
        int problemsPerGroup = minTable == 1 ? maxTable : MathConstants.ReferenceProblemsPerGroupPartialRange;

        var problems = new List<object>();
        for (int t = minTable; t <= maxTable; t++)
        {
            for (int i = 1; i <= problemsPerGroup; i++)
            {
                problems.Add(new { a = t, b = i, ans = t * i });
            }
        }

        var options = new
        {
            type = "table",
            minTable,
            maxTable,
            showAnswers,
            problemsPerGroup,
            problems,
            pdfTitleMultiplicationTables = _loc["PdfTitleMultiplicationTables"].Value,
            pdfTitlePageSuffix = _loc["PdfTitlePageSuffix"].Value,
            siteName = MathConstants.SiteName,
            siteUrl = GetSiteUrl(_navigation)
        };

        await _js.InvokeVoidAsync("MathHelpPdf.download", JsonSerializer.Serialize(options));
    }

    public async Task DownloadPracticePdfAsync(List<List<MultiplicationProblem>> allSheets)
    {
        var sheets = allSheets.Select(sheet =>
            sheet.Select(p => new { a = p.Multiplicand, b = p.Multiplier, ans = p.Answer }).ToList()
        ).ToList();

        var options = new
        {
            type = "practice",
            sheets,
            interleaved = true,
            pdfPracticeSheet = _loc["PdfPracticeSheet"].Value,
            pdfAnswerSheet = _loc["PdfAnswerSheet"].Value,
            pdfSheetNumberSuffix = _loc["PdfSheetNumberSuffix"].Value,
            siteName = MathConstants.SiteName,
            siteUrl = GetSiteUrl(_navigation)
        };

        await _js.InvokeVoidAsync("MathHelpPdf.download", JsonSerializer.Serialize(options));
    }
}
