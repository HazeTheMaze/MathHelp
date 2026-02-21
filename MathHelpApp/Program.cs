using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MathHelpApp.Components;
using MathHelpApp.Resources;
using MathHelpApp.Services;

// Do not set culture here: Blazor WASM can lock resource loading to the initial culture, so setting "en" here causes Swedish (set later in App.razor) to still show English. Culture is set in App.razor from MathHelpCulture.getInitial() before any localized content renders.

// Load both en and sv satellite assemblies so Swedish works after culture switch (Blazor WASM only loads satellites for the initial culture otherwise).
var mainAssembly = typeof(SharedResources).Assembly;
try { mainAssembly.GetSatelliteAssembly(new CultureInfo("en")); } catch { /* en may be in main assembly */ }
try { mainAssembly.GetSatelliteAssembly(new CultureInfo("sv")); } catch { /* ignore if missing */ }

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Do not set ResourcesPath: our .resx live in MathHelpApp.Resources and are embedded as "MathHelpApp.Resources.SharedResources".
// With ResourcesPath = "Resources", the factory looks for "MathHelpApp.Resources.Resources.SharedResources" (wrong).
builder.Services.AddLocalization();
builder.Services.AddScoped<IMultiplicationService, MultiplicationService>();
builder.Services.AddScoped<IBrowserPdfService, BrowserPdfService>();

var host = builder.Build();
// Set culture before RunAsync() so Blazor WASM resource loading uses the correct culture (setting it in App.razor is too late).
try
{
    var js = host.Services.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var culture = await js.InvokeAsync<string>("MathHelpCulture.getInitial", CancellationToken.None, Array.Empty<object>());
    var ci = culture is "sv" or "en" ? new CultureInfo(culture) : new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentCulture = ci;
    CultureInfo.DefaultThreadCurrentUICulture = ci;
}
catch
{
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");
}

await host.RunAsync();
