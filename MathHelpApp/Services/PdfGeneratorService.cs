using MathHelpApp.Constants;
using MathHelpApp.Models;
using MathHelpApp.Services.PdfComponents;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MathHelpApp.Services;

public sealed class PdfGeneratorService : IPdfGeneratorService
{
    private const string AppName = "Mattehjälpen";

    public byte[] GenerateGridTablePdf(int minTable, int maxTable, bool showAnswers)
    {
        var allProblems = GenerateTableProblems(minTable, maxTable);

        var document = Document.Create(container =>
        {
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
                        RenderTablesInColumns(column, allProblems, showAnswers);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(AppName)
                    .FontSize(10);
            });
        });

        return document.GeneratePdf();
    }

    public byte[] GeneratePracticeSheetPdf(List<List<MultiplicationProblem>> allSheets)
    {
        var document = Document.Create(container =>
        {
            foreach (var problems in allSheets)
            {
                // Practice page (no answers)
                AddPracticeSheetPage(container, problems, "Multiplikation - Övningsblad", showAnswers: false);

                // Answer key page
                AddPracticeSheetPage(container, problems, "Facit", showAnswers: true);
            }
        });

        return document.GeneratePdf();
    }

    private static List<(int tableNumber, List<MultiplicationProblem> problems)> GenerateTableProblems(int minTable, int maxTable)
    {
        var allProblems = new List<(int tableNumber, List<MultiplicationProblem> problems)>();

        for (int table = minTable; table <= maxTable; table++)
        {
            var problems = new List<MultiplicationProblem>();
            for (int i = MathConstants.MinTableNumber; i <= maxTable; i++)
            {
                problems.Add(new MultiplicationProblem(table, i));
            }

            allProblems.Add((table, problems));
        }

        return allProblems;
    }

    private static void RenderTablesInColumns(
        ColumnDescriptor column,
        List<(int tableNumber, List<MultiplicationProblem> problems)> allProblems,
        bool showAnswers)
    {
        int rowCount = (int)Math.Ceiling((double)allProblems.Count / MathConstants.ColumnsPerRow);

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            column.Item().Row(row =>
            {
                for (int colIndex = 0; colIndex < MathConstants.ColumnsPerRow; colIndex++)
                {
                    int tableIndex = rowIndex * MathConstants.ColumnsPerRow + colIndex;

                    if (tableIndex < allProblems.Count)
                    {
                        var (tableNumber, problems) = allProblems[tableIndex];
                        row.RelativeItem().Component(new TableListComponent(tableNumber, problems, showAnswers));
                    }
                    else
                    {
                        row.RelativeItem();
                    }

                    if (colIndex < MathConstants.ColumnsPerRow - 1)
                    {
                        row.ConstantItem(8);
                    }
                }
            });
        }
    }

    private static void AddPracticeSheetPage(
        IDocumentContainer container,
        List<MultiplicationProblem> problems,
        string title,
        bool showAnswers)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(1.0f, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(12));

            page.Header()
                .PaddingBottom(10)
                .Text(title)
                .SemiBold()
                .FontSize(20)
                .AlignCenter();

            page.Content()
                .Column(column =>
                {
                    column.Spacing(8);
                    RenderProblemGroups(column, problems, showAnswers);
                });

            page.Footer()
                .AlignCenter()
                .Text(AppName)
                .FontSize(10);
        });
    }

    private static void RenderProblemGroups(
        ColumnDescriptor column,
        List<MultiplicationProblem> problems,
        bool showAnswers)
    {
        int rowCount = (int)Math.Ceiling((double)MathConstants.GroupsPerSheet / MathConstants.ColumnsPerRow);

        for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
        {
            column.Item().Row(row =>
            {
                for (int colIdx = 0; colIdx < MathConstants.ColumnsPerRow; colIdx++)
                {
                    int groupIdx = rowIdx * MathConstants.ColumnsPerRow + colIdx;

                    if (groupIdx < MathConstants.GroupsPerSheet)
                    {
                        var groupProblems = problems
                            .Skip(groupIdx * MathConstants.ProblemsPerGroup)
                            .Take(MathConstants.ProblemsPerGroup)
                            .ToList();

                        int startNumber = groupIdx * MathConstants.ProblemsPerGroup + 1;
                        row.RelativeItem().Component(new PracticeGroupComponent(
                            groupProblems, startNumber, groupIdx + 1, showAnswers));
                    }
                    else
                    {
                        row.RelativeItem();
                    }

                    if (colIdx < MathConstants.ColumnsPerRow - 1)
                    {
                        row.ConstantItem(15);
                    }
                }
            });
        }
    }
}
