using MathHelpApp.Constants;
using MathHelpApp.Models;
using MathHelpApp.Resources;
using MathHelpApp.Services;
using MathHelpApp.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MathHelpApp.Components.Pages;

/// <summary>
/// Home page: reference or practice sheet selection and PDF download.
/// </summary>
public sealed partial class Home
{
    private enum PdfType { Reference, Practice }

    /// <summary>Localized strings for the page.</summary>
    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;

    /// <summary>Used to trigger PDF generation and download in the browser.</summary>
    [Inject]
    public IBrowserPdfService PdfService { get; set; } = null!;

    /// <summary>Used to generate practice problems.</summary>
    [Inject]
    public IMultiplicationService MathService { get; set; } = null!;

    /// <summary>Used for URL/tab sync and navigation.</summary>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <summary>Used to log download failures.</summary>
    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;

    private PdfType _selectedType = PdfType.Reference;
    private bool _showHelp;
    private int _minTable = MathConstants.MinTableNumber;
    private int _maxTable = MathConstants.MaxTableNumber;
    private int _sheetCount = 1;
    private bool _downloading;
    private string? _downloadError;

    /// <inheritdoc />
    protected override void OnInitialized() => InitializeTabFromQuery();

    internal void InitializeTabFromQuery()
    {
        var uri = new Uri(Navigation.Uri);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var tab = query["tab"];
        _selectedType = string.Equals(tab, "practice", StringComparison.OrdinalIgnoreCase)
            ? PdfType.Practice
            : PdfType.Reference;
    }

    private void SelectType(PdfType type)
    {
        _selectedType = type;
        var tabValue = type == PdfType.Practice ? "practice" : "reference";
        var newUri = Navigation.GetUriWithQueryParameter("tab", tabValue);
        Navigation.NavigateTo(newUri, replace: true);
    }

    internal async Task OnDownloadPdf() =>
        await RunDownloadAsync(() => PdfService.DownloadTablePdfAsync(_minTable, _maxTable, showAnswers: true));

    internal async Task OnDownloadPracticeSheetAsync() =>
        await RunDownloadAsync(async () =>
        {
            var allSheets = BuildPracticeSheets();
            await PdfService.DownloadPracticePdfAsync(allSheets);
        });

    private async Task RunDownloadAsync(Func<Task> action)
    {
        _downloadError = null;
        var validationKey = TableRangeValidation.Validate(_minTable, _maxTable);
        if (validationKey is not null)
        {
            SetValidationError(validationKey);
            return;
        }

        _downloading = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "PDF download failed");
            _downloadError = Loc["DownloadFailed"].Value;
        }
        finally
        {
            _downloading = false;
        }
    }

    private void SetValidationError(string validationKey)
    {
        _downloadError = validationKey switch
        {
            "Validation.MinTableRange" => string.Format(
                Loc["Validation.MinTableRange"].Value,
                MathConstants.MinTableNumber,
                MathConstants.MaxTableNumber),
            "Validation.MaxTableRange" => string.Format(
                Loc["Validation.MaxTableRange"].Value,
                MathConstants.MinTableNumber,
                MathConstants.MaxTableNumber),
            _ => Loc[validationKey].Value
        };
    }

    private List<List<MultiplicationProblem>> BuildPracticeSheets()
    {
        int clampedCount = Math.Clamp(_sheetCount, 1, MathConstants.MaxSheetCount);
        var allSheets = new List<List<MultiplicationProblem>>();
        for (int i = 0; i < clampedCount; i++)
        {
            var problems = MathService.GenerateRandomProblems(_minTable, _maxTable, MathConstants.ProblemsPerSheet);
            allSheets.Add(problems);
        }

        return allSheets;
    }
}
