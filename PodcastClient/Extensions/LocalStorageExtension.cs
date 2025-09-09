using PodcastClient.Services;
using System.Globalization;

namespace PodcastClient.Extensions
{
	public static class LocalStorageExtension
	{
		private const string ThemeKey = "PodcastClient-Theme";
		private const string CultureKey = "PodcastClient-Language";

		public static async ValueTask SetTheme(this LocalStorage localStorage, string theme)
		{
			await localStorage.SetItem(ThemeKey, theme);
		}

		public static async ValueTask<string?> GetTheme(this LocalStorage localStorage)
		{
			return await localStorage.GetItem(ThemeKey);
		}

		public static async ValueTask SetCulture(this LocalStorage localStorage, CultureInfo culture)
		{
			await localStorage.SetItem(CultureKey, culture.Name);
		}

		public static async ValueTask<CultureInfo?> GetCulture(this LocalStorage localStorage)
		{
			var culture = await localStorage.GetItem(CultureKey);
			return culture != null ? new CultureInfo(culture) : null;
		}
	}
}
