using System.Text.Json;
using MathHelpApp.Constants;
using MathHelpApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MathHelpApp.Services;

public sealed class BrowserPdfService : IBrowserPdfService
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _navigation;

    public BrowserPdfService(IJSRuntime js, NavigationManager navigation)
    {
        _js = js;
        _navigation = navigation;
    }

    private static string GetSiteUrl(NavigationManager navigation)
    {
        var uri = new Uri(navigation.BaseUri);
        var path = uri.AbsolutePath.TrimEnd('/');
        return uri.GetLeftPart(UriPartial.Authority) + (string.IsNullOrEmpty(path) || path == "/" ? "" : path);
    }

    public async Task DownloadTablePdfAsync(int minTable, int maxTable, bool showAnswers)
    {
        var problems = new List<object>();
        for (int t = minTable; t <= maxTable; t++)
        {
            for (int i = MathConstants.MinTableNumber; i <= MathConstants.MaxTableNumber; i++)
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
            problems,
            siteName = "MathHelp",
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
            siteName = "MathHelp",
            siteUrl = GetSiteUrl(_navigation)
        };

        await _js.InvokeVoidAsync("MathHelpPdf.download", JsonSerializer.Serialize(options));
    }

    public async Task DownloadPracticeAnswerSheetPdfAsync(List<List<MultiplicationProblem>> allSheets)
    {
        var sheets = allSheets.Select(sheet =>
            sheet.Select(p => new { a = p.Multiplicand, b = p.Multiplier, ans = p.Answer }).ToList()
        ).ToList();

        var options = new
        {
            type = "practice",
            sheets,
            showAnswers = true,
            siteName = "MathHelp",
            siteUrl = GetSiteUrl(_navigation)
        };

        await _js.InvokeVoidAsync("MathHelpPdf.download", JsonSerializer.Serialize(options));
    }
}
