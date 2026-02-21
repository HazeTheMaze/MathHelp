using MathHelpApp.Models;

namespace MathHelpApp.Services;

/// <summary>
/// Generates and triggers download of PDFs in the browser via JavaScript (e.g. jsPDF).
/// </summary>
public interface IBrowserPdfService
{
    /// <summary>
    /// Generates and downloads a times tables reference PDF.
    /// </summary>
    Task DownloadTablePdfAsync(int minTable, int maxTable, bool showAnswers);

    /// <summary>
    /// Generates and downloads practice sheet PDF(s) with the given problem sets.
    /// </summary>
    Task DownloadPracticePdfAsync(List<List<MultiplicationProblem>> allSheets);

    /// <summary>
    /// Generates and downloads the answer sheet for the given practice sheets (same layout, answers shown).
    /// </summary>
    Task DownloadPracticeAnswerSheetPdfAsync(List<List<MultiplicationProblem>> allSheets);
}
