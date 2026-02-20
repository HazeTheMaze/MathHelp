using MathHelpApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MathHelpApp.Services.PdfComponents;

/// <summary>
/// QuestPDF component for rendering a multiplication table list.
/// </summary>
public sealed class TableListComponent : IComponent
{
    private readonly int _tableNumber;
    private readonly List<MultiplicationProblem> _problems;
    private readonly bool _showAnswers;

    public TableListComponent(int tableNumber, List<MultiplicationProblem> problems, bool showAnswers)
    {
        _tableNumber = tableNumber;
        _problems = problems;
        _showAnswers = showAnswers;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item()
                .PaddingBottom(2)
                .Text($"Tabell {_tableNumber}")
                .SemiBold()
                .FontSize(10)
                .FontColor(PdfTheme.AccentGold);

            column.Item().Padding(1).Column(innerColumn =>
            {
                innerColumn.Spacing(0);

                foreach (var problem in _problems)
                {
                    var multiplicandStr = problem.Multiplicand.ToString().PadLeft(2);
                    var multiplierStr = problem.Multiplier.ToString().PadLeft(2);
                    var answerText = _showAnswers ? problem.Answer.ToString().PadLeft(3) : "___";

                    innerColumn.Item().Text($"{multiplicandStr} x {multiplierStr} = {answerText}")
                        .FontFamily(Fonts.Courier)
                        .FontSize(10);
                }
            });

            column.Item().PaddingBottom(3);
        });
    }
}
