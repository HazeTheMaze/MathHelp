using MathHelpApp.Models;

namespace MathHelpApp.Services;

public interface IPdfGeneratorService
{
    /// <summary>
    /// Generates a times tables reference PDF with all answers.
    /// </summary>
    byte[] GenerateGridTablePdf(int minTable, int maxTable, bool showAnswers);

    /// <summary>
    /// Generates practice sheet PDFs with problems arranged in groups of 10.
    /// Each sheet has 100 problems with answer key on the back.
    /// </summary>
    byte[] GeneratePracticeSheetPdf(List<List<MultiplicationProblem>> allSheets);
}
