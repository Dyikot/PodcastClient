using System.Globalization;

namespace PodcastClient.Components.Pages.Settings
{
    public partial class Settings
    {
		public CultureInfo Culture
		{
			get => CultureInfo.CurrentCulture;
			set
			{
				if (CultureInfo.CurrentCulture != value)
				{
					var uri = new Uri(Navigator.Uri).GetComponents(UriComponents.PathAndQuery,
																   UriFormat.Unescaped);
					var cultureEscaped = Uri.EscapeDataString(value.Name);
					var uriEscaped = Uri.EscapeDataString(uri);

					Navigator.NavigateTo(
						uri: $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
						forceLoad: true
					);
				}
			}
		}
	}
}
