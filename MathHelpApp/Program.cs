using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MathHelpApp.Components;
using MathHelpApp.Resources;
using MathHelpApp.Services;

// Default to English so resource lookup finds SharedResources.en.resx (avoids showing keys).
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");

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

await builder.Build().RunAsync();
