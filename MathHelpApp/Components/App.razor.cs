namespace MathHelpApp.Components;

/// <summary>
/// Culture is set once at startup in Program.cs (getInitial + setLang). This component only gates rendering until ready.
/// </summary>
public sealed partial class App
{
    private bool _cultureReady;

    protected override void OnInitialized()
    {
        _cultureReady = true;
    }
}
