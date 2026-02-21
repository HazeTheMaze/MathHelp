using Microsoft.AspNetCore.Components;
using MathHelpApp.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace MathHelpApp.Components.Shared;

public sealed partial class CultureSelector
{
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;

    private bool _isOpen;

    private void Toggle() => _isOpen = !_isOpen;

    private async Task SetCultureAsync(string culture)
    {
        _isOpen = false;
        await Js.InvokeVoidAsync("MathHelpCulture.set", CancellationToken.None, culture);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}
