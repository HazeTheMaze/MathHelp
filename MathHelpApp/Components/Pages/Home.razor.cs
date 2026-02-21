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

    private static Dictionary<string, string> ParseQueryString(string query)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(query) || query.Length < 2)
        {
            return dict;
        }

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

        return dict;
    }

    private string? GetQueryParam(string name)
    {
        var uri = new Uri(Navigation.Uri);
        var dict = ParseQueryString(uri.Query);
        return dict.TryGetValue(name, out var value) ? value : null;
    }

    private void SetQueryParam(string name, string value)
    {
        var uri = new Uri(Navigation.Uri);
        var dict = ParseQueryString(uri.Query);
        dict[name] = value;
        var newQuery = string.Join(
            '&',
            dict.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        var pathPart = uri.GetLeftPart(UriPartial.Path);
        var path = (pathPart.Length > 1 && pathPart.EndsWith('/'))
            ? pathPart
            : pathPart.TrimEnd('/');
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
        var validationKey = TableRangeValidation.Validate(_minTable, _maxTable);
        if (validationKey is not null)
        {
            SetValidationError(validationKey);
            return;
        }
        _downloading = true;
        try
        {
            await PdfService.DownloadTablePdfAsync(_minTable, _maxTable, showAnswers: true);
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

    internal async Task OnDownloadPracticeSheetAsync()
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
            var allSheets = BuildPracticeSheets();
            await PdfService.DownloadPracticePdfAsync(allSheets);
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
        var allSheets = new List<List<MultiplicationProblem>>();
        for (int i = 0; i < _sheetCount; i++)
        {
            var problems = MathService.GenerateRandomProblems(_minTable, _maxTable, MathConstants.ProblemsPerSheet);
            allSheets.Add(problems);
        }
        return allSheets;
    }
}
