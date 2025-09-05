using PodcastClient.Models;
using System.Globalization;

namespace PodcastClient.Components.Pages.Settings
{
    public partial class Settings
    {
		public CultureInfo[] LanguageOptions { get; } = 
		[
			new CultureInfo("en-US"), 
			new CultureInfo("ru-RU")
		];

		public string[] ThemeOptions { get; } =
		[
			ThemeService.Light,
			ThemeService.Dark
		];

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

		public string GetLanguageValue(CultureInfo option) => option.Name switch
		{
			"ru-RU" => "Русский",
			"en-US" => "English",
			_ => throw new NotSupportedException()
		};
	}
}
