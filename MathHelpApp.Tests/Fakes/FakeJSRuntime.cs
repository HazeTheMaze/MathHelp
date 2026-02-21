using Microsoft.JSInterop;

namespace MathHelpApp.Tests.Fakes;

/// <summary>
/// Test double for IJSRuntime that captures the last JSON argument passed to InvokeAsync
/// (used when code calls the InvokeVoidAsync extension, which forwards to InvokeAsync).
/// </summary>
public sealed class FakeJsRuntime : IJSRuntime
{
    public string? LastInvokeJson { get; private set; }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        CaptureIfPdfDownload(identifier, args);
        return default;
    }

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        CaptureIfPdfDownload(identifier, args);
        return default;
    }

    private void CaptureIfPdfDownload(string identifier, object?[]? args)
    {
        if (identifier == "MathHelpPdf.download" && args is { Length: > 0 })
        {
            LastInvokeJson = args[0]?.ToString();
        }
    }
}
