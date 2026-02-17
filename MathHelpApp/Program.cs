using MathHelpApp.Components;
using MathHelpApp.Services;
using QuestPDF.Infrastructure;
using System.Diagnostics;

// Configure QuestPDF license (free for personal/educational use)
QuestPDF.Settings.License = LicenseType.Community;

// Check if already running - if so, just open browser and exit
Mutex? mutex = null;
try
{
    mutex = new Mutex(true, "MattehjalpenAppMutex", out bool isNewInstance);
    if (!isNewInstance)
    {
        // App is already running, just open the browser
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "http://localhost:5000",
                UseShellExecute = true
            });
        }
        catch { }
        return; // Exit this instance
    }
}
catch
{
    // If mutex creation fails, continue anyway
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - static server-side rendering only
builder.Services.AddRazorComponents();

// Register multiplication and PDF services
builder.Services.AddScoped<IMultiplicationService, MultiplicationService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

// API endpoint for grid multiplication table PDF
app.MapGet("/api/pdf/table", (
    int minTable,
    int maxTable,
    bool showAnswers,
    IPdfGeneratorService pdfService) =>
{
    var pdfBytes = pdfService.GenerateGridTablePdf(minTable, maxTable, showAnswers);
    var fileName = showAnswers ? "multiplication-table-answers.pdf" : "multiplication-table.pdf";
    return Results.File(pdfBytes, "application/pdf", fileName);
}).DisableAntiforgery();

// API endpoint for random practice sheet PDF
app.MapGet("/api/pdf/practice", (
    int minTable,
    int maxTable,
    int sheetCount,
    IMultiplicationService mathService,
    IPdfGeneratorService pdfService) =>
{
    var allSheets = new List<List<MathHelpApp.Models.MultiplicationProblem>>();
    for (int i = 0; i < sheetCount; i++)
    {
        var problems = mathService.GenerateRandomProblems(minTable, maxTable, 100);
        allSheets.Add(problems);
    }
    
    var pdfBytes = pdfService.GeneratePracticeSheetPdf(allSheets);
    var fileName = sheetCount > 1 ? $"practice-sheets-{sheetCount}.pdf" : "practice-sheet.pdf";
    return Results.File(pdfBytes, "application/pdf", fileName);
}).DisableAntiforgery();

app.MapRazorComponents<App>();

app.MapStaticAssets();

// Open browser automatically when app starts
app.Lifetime.ApplicationStarted.Register(() =>
{
    var url = "http://localhost:5000";
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch
    {
        // Ignore if browser fails to open
    }
});

app.Run();
