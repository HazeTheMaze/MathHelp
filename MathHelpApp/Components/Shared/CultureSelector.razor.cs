using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MathHelpApp.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

namespace MathHelpApp.Components.Shared;

/// <summary>
/// UI for switching between supported cultures (e.g. EN/SV).
/// </summary>
public sealed partial class CultureSelector : IAsyncDisposable
{
    /// <summary>Used to reload the page after culture change.</summary>
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    /// <summary>Used to call MathHelpCulture.set for persisting culture.</summary>
    [Inject]
    public IJSRuntime Js { get; set; } = null!;

    /// <summary>Localized strings for the selector.</summary>
    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;

    private bool _isOpen;
    private DotNetObjectReference<CultureSelector>? _dotNetRef;
    private bool _isDisposed;

    private void Toggle() => _isOpen = !_isOpen;

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape" && _isOpen)
        {
            _isOpen = false;
        }
    }

    /// <summary>Called from JavaScript when a click occurs outside the component.</summary>
    [JSInvokable]
    public void CloseFromJavaScript()
    {
        if (_isDisposed)
        {
            return;
        }

        _isOpen = false;
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            await Js.InvokeVoidAsync("MathHelpDropdown.registerClickOutside", _dotNetRef, ".culture-selector");
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        _isDisposed = true;
        if (_dotNetRef is not null)
        {
            try
            {
                await Js.InvokeVoidAsync("MathHelpDropdown.unregisterClickOutside", ".culture-selector");
            }
            catch
            {
                // Ignore errors during cleanup
            }
            finally
            {
                _dotNetRef.Dispose();
            }
        }
    }

    private async Task SetCultureAsync(string culture)
    {
        _isOpen = false;
        await Js.InvokeVoidAsync("MathHelpCulture.set", CancellationToken.None, culture);
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }
}
