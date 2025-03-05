using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PodcastClient.Components.Pages.Settings
{
    public partial class LocaleSelector
    {
        private CultureInfo Culture
        {
            get => CultureInfo.CurrentCulture;
            set
            {
                if (CultureInfo.CurrentCulture != value)
                {
                    var uri = new Uri(Navigation.Uri).GetComponents(UriComponents.PathAndQuery,
                                                                    UriFormat.Unescaped);
                    var cultureEscaped = Uri.EscapeDataString(value.Name);
                    var uriEscaped = Uri.EscapeDataString(uri);

                    Navigation.NavigateTo(
                        uri: $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
                        forceLoad: true
                    );
                }
            }
        }
    }
}
