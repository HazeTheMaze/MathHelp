using System.Text.Json;
using MathHelpApp.Constants;
using MathHelpApp.Models;
using Microsoft.JSInterop;

namespace MathHelpApp.Services;

public sealed class BrowserPdfService : IBrowserPdfService
{
    private readonly IJSRuntime _js;

    public BrowserPdfService(IJSRuntime js)
    {
        _js = js;
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
            problems
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
            sheets
        };

        await _js.InvokeVoidAsync("MathHelpPdf.download", JsonSerializer.Serialize(options));
    }
}
