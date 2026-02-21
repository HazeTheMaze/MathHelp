using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MathHelpApp.Components;

public sealed partial class App
{
    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    private bool _cultureReady;

    protected override async Task OnInitializedAsync()
    {
        string? culture;
        try
        {
            culture = await Js.InvokeAsync<string>("MathHelpCulture.getInitial");
        }
        catch
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en");
            await Js.InvokeVoidAsync("MathHelpCulture.setLang", "en");
            _cultureReady = true;
            return;
        }
        var ci = culture is "sv" or "en"
            ? new CultureInfo(culture)
            : new CultureInfo("en");
        CultureInfo.DefaultThreadCurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentUICulture = ci;
        await Js.InvokeVoidAsync("MathHelpCulture.setLang", ci.TwoLetterISOLanguageName);
        _cultureReady = true;
    }
}
