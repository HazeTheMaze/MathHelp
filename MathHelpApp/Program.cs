using System.Diagnostics;
using MathHelpApp.Components;
using MathHelpApp.Endpoints;
using MathHelpApp.Services;
using QuestPDF.Infrastructure;

// Configure QuestPDF license (free for personal/educational use)
QuestPDF.Settings.License = LicenseType.Community;

// Read the configured URL from appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var appUrl = configuration["Kestrel:Endpoints:Http:Url"] ?? "http://localhost:5000";

// Single-instance enforcement using a named mutex:
// - If this is the first instance, continue starting the app
// - If another instance is already running, open the browser and exit
using var mutex = new Mutex(false, "MattehjalpenAppMutex", out bool isNewInstance);
if (!isNewInstance)
{
    OpenBrowser(appUrl);
    return; // Exit this instance
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - static server-side rendering only
builder.Services.AddRazorComponents();

// Register multiplication and PDF services
builder.Services.AddScoped<IMultiplicationService, MultiplicationService>();
builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

// Map API endpoints
app.MapPdfEndpoints();

// Map Blazor components and static assets
app.MapRazorComponents<App>();
app.MapStaticAssets();

// Open browser automatically when app starts
app.Lifetime.ApplicationStarted.Register(() => OpenBrowser(appUrl));

await app.RunAsync();

// Opens the default browser to the specified URL.
static void OpenBrowser(string url)
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true,
        });
    }
    catch (Exception ex)
    {
        // Browser launch is non-critical - log for debugging but don't fail the app
        Debug.WriteLine($"Failed to open browser: {ex.Message}");
    }
}
