using System.Diagnostics;

namespace MathHelpApp.Components.Pages;

/// <summary>
/// Displays error details when an unhandled exception occurs.
/// </summary>
public sealed partial class Error
{
    private string? RequestId { get; set; }
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <inheritdoc />
    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id;
}
