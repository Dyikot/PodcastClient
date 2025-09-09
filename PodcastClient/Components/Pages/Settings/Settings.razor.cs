using PodcastClient.Extensions;
using PodcastClient.Services;
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

		private async Task OnThemeChanged(string theme)
		{
			await LocalStorage.SetTheme(theme);
			Navigator.Refresh();
		}

		private async Task OnCultureChanged(CultureInfo culture)
		{
			await LocalStorage.SetCulture(culture);
		}

		private string GetLanguageLabel(CultureInfo option) => option.Name switch
		{
			"ru-RU" => "Русский",
			"en-US" => "English",
			_ => throw new NotSupportedException()
		};
	}
}
