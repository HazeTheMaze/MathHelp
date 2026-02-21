using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MathHelpApp.Components;
using MathHelpApp.Resources;
using MathHelpApp.Services;

// Preload satellite assemblies if present; ignore if not (e.g. single culture deployment).
var mainAssembly = typeof(SharedResources).Assembly;
try
{
    mainAssembly.GetSatelliteAssembly(new CultureInfo("en"));
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Satellite assembly preload skipped: {ex.Message}");
}
try
{
    mainAssembly.GetSatelliteAssembly(new CultureInfo("sv"));
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Satellite assembly preload skipped: {ex.Message}");
}

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
builder.Services.AddScoped<IMultiplicationService, MultiplicationService>();
builder.Services.AddScoped<IBrowserPdfService, BrowserPdfService>();

var host = builder.Build();
var js = host.Services.GetRequiredService<IJSRuntime>();

try
{
    var culture = await js.InvokeAsync<string>("MathHelpCulture.getInitial", CancellationToken.None, Array.Empty<object>());
    var ci = culture is "sv" or "en"
        ? new CultureInfo(culture)
        : new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentCulture = ci;
    CultureInfo.DefaultThreadCurrentUICulture = ci;
    await js.InvokeVoidAsync("MathHelpCulture.setLang", CancellationToken.None, ci.TwoLetterISOLanguageName);
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>()
        .CreateLogger("MathHelp.Program");
    logger.LogWarning(ex, "MathHelpCulture.getInitial failed; using default culture 'en'");
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");
    try
    {
        await js.InvokeVoidAsync("MathHelpCulture.setLang", CancellationToken.None, "en");
    }
    catch (Exception setLangEx)
    {
        logger.LogWarning(setLangEx, "MathHelpCulture.setLang failed in fallback; culture already set to 'en'");
    }
}

await host.RunAsync();
