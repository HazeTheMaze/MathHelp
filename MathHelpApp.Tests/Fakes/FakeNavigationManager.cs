using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MathHelpApp.Tests.Fakes;

/// <summary>
/// Test double for NavigationManager that allows setting BaseUri via constructor.
/// </summary>
public sealed class FakeNavigationManager : NavigationManager
{
    [SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "Test helper; string URIs are convenient for tests.")]
    public FakeNavigationManager(string baseUri = "https://example.com/", string? uri = null)
    {
        Initialize(baseUri, uri ?? baseUri);
    }
}
