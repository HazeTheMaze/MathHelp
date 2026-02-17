using MathHelpApp.Constants;
using MathHelpApp.Models;
using MathHelpApp.Services;

namespace MathHelpApp.Endpoints;

/// <summary>
/// Extension methods to register PDF generation API endpoints.
/// </summary>
public static class PdfEndpoints
{
    /// <summary>
    /// Maps all PDF generation endpoints to the application.
    /// </summary>
    public static WebApplication MapPdfEndpoints(this WebApplication app)
    {
        app.MapGet("/api/pdf/table", HandleTablePdf).DisableAntiforgery();
        app.MapGet("/api/pdf/practice", HandlePracticePdf).DisableAntiforgery();

        return app;
    }

    private static IResult HandleTablePdf(
        int minTable,
        int maxTable,
        bool showAnswers,
        IPdfGeneratorService pdfService)
    {
        var validationError = ValidateTableRange(minTable, maxTable);
        if (validationError is not null)
        {
            return validationError;
        }

        var pdfBytes = pdfService.GenerateGridTablePdf(minTable, maxTable, showAnswers);
        var fileName = showAnswers ? "multiplication-table-answers.pdf" : "multiplication-table.pdf";
        return Results.File(pdfBytes, "application/pdf", fileName);
    }

    private static IResult HandlePracticePdf(
        int minTable,
        int maxTable,
        int sheetCount,
        IMultiplicationService mathService,
        IPdfGeneratorService pdfService)
    {
        var validationError = ValidateTableRange(minTable, maxTable);
        if (validationError is not null)
        {
            return validationError;
        }

        if (sheetCount < 1 || sheetCount > MathConstants.MaxSheetCount)
        {
            return Results.BadRequest($"sheetCount must be between 1 and {MathConstants.MaxSheetCount}");
        }

        var allSheets = new List<List<MultiplicationProblem>>();
        for (int i = 0; i < sheetCount; i++)
        {
            var problems = mathService.GenerateRandomProblems(minTable, maxTable, MathConstants.ProblemsPerSheet);
            allSheets.Add(problems);
        }

        var pdfBytes = pdfService.GeneratePracticeSheetPdf(allSheets);
        var fileName = sheetCount > 1 ? $"practice-sheets-{sheetCount}.pdf" : "practice-sheet.pdf";
        return Results.File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Validates that minTable and maxTable are within allowed range.
    /// Returns null if valid, or a BadRequest result if invalid.
    /// </summary>
    internal static IResult? ValidateTableRange(int minTable, int maxTable)
    {
        if (minTable < MathConstants.MinTableNumber || minTable > MathConstants.MaxTableNumber ||
            maxTable < MathConstants.MinTableNumber || maxTable > MathConstants.MaxTableNumber)
        {
            return Results.BadRequest($"Tables must be between {MathConstants.MinTableNumber} and {MathConstants.MaxTableNumber}");
        }

        if (minTable > maxTable)
        {
            return Results.BadRequest("minTable cannot be greater than maxTable");
        }

        return null;
    }
}
