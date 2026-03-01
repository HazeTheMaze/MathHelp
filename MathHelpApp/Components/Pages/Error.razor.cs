using MathHelpApp.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MathHelpApp.Components.Pages;

/// <summary>
/// Displays error details when an unhandled exception occurs.
/// </summary>
public sealed partial class Error
{
    /// <summary>Localized strings for the page.</summary>
    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;
}
