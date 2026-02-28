using MathHelpApp.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace MathHelpApp.Components.Pages;

/// <summary>
/// 404 not found page.
/// </summary>
public sealed partial class NotFound
{
    /// <summary>Localized strings for the page.</summary>
    [Inject]
    public IStringLocalizer<SharedResources> Loc { get; set; } = null!;
}
