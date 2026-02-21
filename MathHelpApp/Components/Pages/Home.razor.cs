using MathHelpApp.Constants;
using MathHelpApp.Models;
using MathHelpApp.Resources;
using MathHelpApp.Services;
using MathHelpApp.Validation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MathHelpApp.Components.Pages;

public sealed partial class Home
{
    private enum PdfType { Reference, Practice }

    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;

    [Inject]
    public IBrowserPdfService PdfService { get; set; } = null!;

    [Inject]
    public IMultiplicationService MathService { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public ILogger<Home> Logger { get; set; } = null!;

    private PdfType _selectedType = PdfType.Reference;
    private bool _showHelp;
    private int _minTable = 1;
    private int _maxTable = 12;
    private int _sheetCount = 1;
    private bool _downloading;
    private string? _downloadError;

    protected override void OnInitialized() => InitializeTabFromQuery();

    internal void InitializeTabFromQuery()
    {
        var tab = GetQueryParam("tab");
        if (string.Equals(tab, "practice", StringComparison.OrdinalIgnoreCase))
        {
            _selectedType = PdfType.Practice;
        }
        else
        {
            _selectedType = PdfType.Reference;
        }
    }

    private string? GetQueryParam(string name)
    {
        var uri = new Uri(Navigation.Uri);
        var query = uri.Query;
        if (string.IsNullOrEmpty(query) || query.Length < 2)
        {
            return null;
        }

        foreach (var pair in query[1..].Split('&'))
        {
            var eq = pair.IndexOf('=', StringComparison.Ordinal);
            if (eq > 0
                && string.Equals(
                    Uri.UnescapeDataString(pair[..eq].Trim()),
                    name,
                    StringComparison.OrdinalIgnoreCase))
            {
                return eq < pair.Length - 1 ? Uri.UnescapeDataString(pair[(eq + 1)..].Trim()) : "";
            }
        }

        return null;
    }

    private void SetQueryParam(string name, string value)
    {
        var uri = new Uri(Navigation.Uri);
        var query = uri.Query;
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrEmpty(query) && query.Length > 1)
        {
            foreach (var pair in query[1..].Split('&'))
            {
                var eq = pair.IndexOf('=', StringComparison.Ordinal);
                if (eq > 0)
                {
                    dict[Uri.UnescapeDataString(pair[..eq].Trim())] =
                        eq < pair.Length - 1 ? Uri.UnescapeDataString(pair[(eq + 1)..].Trim()) : "";
                }
                else if (!string.IsNullOrWhiteSpace(pair))
                {
                    dict[Uri.UnescapeDataString(pair.Trim())] = "";
                }
            }
        }

        dict[name] = value;
        var newQuery = string.Join(
            '&',
            dict.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        var path = uri.GetLeftPart(UriPartial.Path).TrimEnd('/');
        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }

        var newUrl = string.IsNullOrEmpty(newQuery) ? path : path + "?" + newQuery;
        Navigation.NavigateTo(newUrl, replace: true);
    }

    private void SelectType(PdfType type)
    {
        _selectedType = type;
        SetQueryParam("tab", type == PdfType.Practice ? "practice" : "reference");
    }

    internal async Task OnDownloadPdf()
    {
        _downloadError = null;
        var validationError = TableRangeValidation.Validate(_minTable, _maxTable);
        if (validationError is not null)
        {
            _downloadError = validationError;
            return;
        }
        _downloading = true;
        try
        {
            if (_selectedType == PdfType.Reference)
            {
                await PdfService.DownloadTablePdfAsync(_minTable, _maxTable, showAnswers: true);
            }
            else
            {
                var allSheets = new List<List<MultiplicationProblem>>();
                for (int i = 0; i < _sheetCount; i++)
                {
                    var problems = MathService.GenerateRandomProblems(_minTable, _maxTable, MathConstants.ProblemsPerSheet);
                    allSheets.Add(problems);
                }
                await PdfService.DownloadPracticePdfAsync(allSheets);
                await PdfService.DownloadPracticeAnswerSheetPdfAsync(allSheets);
            }
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
}
