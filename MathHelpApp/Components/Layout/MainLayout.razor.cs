using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace MathHelpApp.Components.Layout;

public sealed partial class MainLayout
{
    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        var uri = Navigation.Uri;
        var queryStart = uri.IndexOf('?', StringComparison.Ordinal);
        if (queryStart < 0)
        {
            return;
        }

        var query = uri[(queryStart + 1)..];
        foreach (var pair in query.Split('&'))
        {
            var kv = pair.Split('=');
            if (kv.Length != 2 ||
                !string.Equals(Uri.UnescapeDataString(kv[0].Trim()), "culture", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var culture = Uri.UnescapeDataString(kv[1].Trim());
            if (culture is "sv" or "en")
            {
                var ci = new CultureInfo(culture);
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
            }
            break;
        }
    }
}
