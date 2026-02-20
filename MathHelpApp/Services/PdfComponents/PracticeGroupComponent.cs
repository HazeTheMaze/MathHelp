using MathHelpApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MathHelpApp.Services.PdfComponents;

/// <summary>
/// QuestPDF component for rendering a group of practice problems.
/// </summary>
public sealed class PracticeGroupComponent : IComponent
{
    private readonly List<MultiplicationProblem> _problems;
    private readonly int _startNumber;
    private readonly int _groupNumber;
    private readonly bool _showAnswers;

    public PracticeGroupComponent(List<MultiplicationProblem> problems, int startNumber, int groupNumber, bool showAnswers)
    {
        _problems = problems;
        _startNumber = startNumber;
        _groupNumber = groupNumber;
        _showAnswers = showAnswers;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item()
                .PaddingBottom(3)
                .Text($"Grupp {_groupNumber}")
                .SemiBold()
                .FontSize(12)
                .FontColor(PdfTheme.AccentGold);

            for (int i = 0; i < _problems.Count; i++)
            {
                var problem = _problems[i];
                var problemNumber = _startNumber + i;

                var numStr = problemNumber.ToString().PadLeft(3);
                var multiplicandStr = problem.Multiplicand.ToString().PadLeft(2);
                var multiplierStr = problem.Multiplier.ToString().PadLeft(2);
                var answerText = _showAnswers ? problem.Answer.ToString().PadLeft(3) : "___";

                column.Item().Text($"{numStr}.  {multiplicandStr} x {multiplierStr} = {answerText}")
                    .FontFamily(Fonts.Courier)
                    .FontSize(12);
            }
        });
    }
}
