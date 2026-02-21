using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MathHelpApp.Components;
using MathHelpApp.Resources;
using MathHelpApp.Services;

var mainAssembly = typeof(SharedResources).Assembly;
try { mainAssembly.GetSatelliteAssembly(new CultureInfo("en")); } catch { }
try { mainAssembly.GetSatelliteAssembly(new CultureInfo("sv")); } catch { }

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
builder.Services.AddScoped<IMultiplicationService, MultiplicationService>();
builder.Services.AddScoped<IBrowserPdfService, BrowserPdfService>();

var host = builder.Build();

try
{
    var js = host.Services.GetRequiredService<Microsoft.JSInterop.IJSRuntime>();
    var culture = await js.InvokeAsync<string>("MathHelpCulture.getInitial", CancellationToken.None, Array.Empty<object>());
    var ci = culture is "sv" or "en"
        ? new CultureInfo(culture)
        : new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentCulture = ci;
    CultureInfo.DefaultThreadCurrentUICulture = ci;
}
catch
{
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");
}

await host.RunAsync();
