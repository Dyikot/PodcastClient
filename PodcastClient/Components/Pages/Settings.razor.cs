using PodcastClient.Data;
using PodcastClient.Services;
using System.Globalization;

namespace PodcastClient.Components.Pages
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
			await LocalStorage.SetItem(LocalStorageKeys.Theme, theme);
		}

		private string GetLanguageLabel(CultureInfo culture) => culture.Name switch
		{
			"ru-RU" => "Русский",
			"en-US" => "English",
			_ => throw new NotSupportedException()
		};

		//
	}
}
