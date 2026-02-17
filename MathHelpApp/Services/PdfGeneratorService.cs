using MathHelpApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MathHelpApp.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    public byte[] GenerateGridTablePdf(int minTable, int maxTable, bool showAnswers)
    {
        // Generate problems for each table - multiplication range matches table range
        var allProblems = new List<(int tableNumber, List<MultiplicationProblem> problems)>();
        for (int table = minTable; table <= maxTable; table++)
        {
            var problems = new List<MultiplicationProblem>();
            for (int i = 1; i <= maxTable; i++)  // Use maxTable as upper limit
            {
                problems.Add(new MultiplicationProblem(table, i));
            }
            allProblems.Add((table, problems));
        }

        var document = Document.Create(container =>
        {
            // Single page: Times tables reference with answers
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0.7f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .PaddingBottom(6)
                    .Text($"Multiplikationstabeller ({minTable} - {maxTable})")
                    .SemiBold()
                    .FontSize(16)
                    .AlignCenter();

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(2);

                        // Display tables in 3 columns
                        var tablesPerRow = 3;
                        for (int rowIndex = 0; rowIndex < Math.Ceiling((double)allProblems.Count / tablesPerRow); rowIndex++)
                        {
                            column.Item().Row(row =>
                            {
                                for (int colIndex = 0; colIndex < tablesPerRow; colIndex++)
                                {
                                    var tableIndex = rowIndex * tablesPerRow + colIndex;
                                    if (tableIndex < allProblems.Count)
                                    {
                                        var (tableNumber, problems) = allProblems[tableIndex];
                                        row.RelativeItem().Component(new TableListComponent(tableNumber, problems, true));
                                    }
                                    else
                                    {
                                        row.RelativeItem();
                                    }

                                    if (colIndex < tablesPerRow - 1)
                                    {
                                        row.ConstantItem(8);
                                    }
                                }
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("Mattehjälpen")
                    .FontSize(10);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GeneratePracticeSheetPdf(List<List<MultiplicationProblem>> allSheets)
    {
        var document = Document.Create(container =>
        {
            for (int sheetIndex = 0; sheetIndex < allSheets.Count; sheetIndex++)
            {
                var problems = allSheets[sheetIndex];

                // Practice page (no answers)
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.0f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .PaddingBottom(10)
                        .Text("Multiplikation - Övningsblad")
                        .SemiBold()
                        .FontSize(20)
                        .AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(8);

                            // 10 groups in rows of 3: (1,2,3), (4,5,6), (7,8,9), (10)
                            for (int rowIdx = 0; rowIdx < 4; rowIdx++)
                            {
                                column.Item().Row(row =>
                                {
                                    for (int colIdx = 0; colIdx < 3; colIdx++)
                                    {
                                        var groupIdx = rowIdx * 3 + colIdx;
                                        if (groupIdx < 10)
                                        {
                                            var groupProblems = problems.Skip(groupIdx * 10).Take(10).ToList();
                                            row.RelativeItem().Component(new PracticeGroupComponent(
                                                groupProblems, groupIdx * 10 + 1, groupIdx + 1, false));
                                        }
                                        else
                                        {
                                            row.RelativeItem();
                                        }

                                        if (colIdx < 2)
                                        {
                                            row.ConstantItem(15);
                                        }
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Mattehjälpen")
                        .FontSize(10);
                });

                // Answer key page (same layout with answers)
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.0f, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .PaddingBottom(10)
                        .Text("Facit")
                        .SemiBold()
                        .FontSize(20)
                        .AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(8);

                            // 10 groups in rows of 3: (1,2,3), (4,5,6), (7,8,9), (10)
                            for (int rowIdx = 0; rowIdx < 4; rowIdx++)
                            {
                                column.Item().Row(row =>
                                {
                                    for (int colIdx = 0; colIdx < 3; colIdx++)
                                    {
                                        var groupIdx = rowIdx * 3 + colIdx;
                                        if (groupIdx < 10)
                                        {
                                            var groupProblems = problems.Skip(groupIdx * 10).Take(10).ToList();
                                            row.RelativeItem().Component(new PracticeGroupComponent(
                                                groupProblems, groupIdx * 10 + 1, groupIdx + 1, true));
                                        }
                                        else
                                        {
                                            row.RelativeItem();
                                        }

                                        if (colIdx < 2)
                                        {
                                            row.ConstantItem(15);
                                        }
                                    }
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text("Mattehjälpen")
                        .FontSize(10);
                });
            }
        });

        return document.GeneratePdf();
    }
}

// Component for a single group of 10 problems
public class PracticeGroupComponent : IComponent
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
            // Group header
            column.Item()
                .PaddingBottom(3)
                .Text($"Grupp {_groupNumber}")
                .SemiBold()
                .FontSize(12);

            // Problems in this group
            for (int i = 0; i < _problems.Count; i++)
            {
                var problem = _problems[i];
                var problemNumber = _startNumber + i;
                // Format: "  1.   3 x  5 = ____" with proper alignment
                // Problem number: 3 chars right-aligned, followed by dot and spaces
                // Multiplicand: 2 chars right-aligned
                // Multiplier: 2 chars right-aligned  
                // Answer: 3 chars right-aligned or ____
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

// Component for times tables list-style display (no borders)
public class TableListComponent : IComponent
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
                .FontSize(10);

            column.Item().Padding(1).Column(innerColumn =>
            {
                innerColumn.Spacing(0);

                foreach (var problem in _problems)
                {
                    // Format: " 3 x  5 =  15" with proper alignment
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
