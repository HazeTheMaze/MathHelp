using MathHelpApp.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MathHelpApp.Components.Layout;

/// <summary>
/// Main navigation menu for the application.
/// </summary>
public sealed partial class NavMenu
{
    /// <summary>Localized strings for the menu.</summary>
    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;
}
