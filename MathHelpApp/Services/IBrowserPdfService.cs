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
    /// Generates and downloads a single practice PDF with interleaved challenge and answer pages (1 challenge, 1 answer, 2 challenge, 2 answer, ...).
    /// </summary>
    Task DownloadPracticePdfAsync(List<List<MultiplicationProblem>> allSheets);
}
