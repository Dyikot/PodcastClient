using Microsoft.Extensions.Localization;

namespace PodcastClient.Extensions
{
	public static class TimeSpanExtension
	{
		public static string ToLocalizedString(this TimeSpan timeSpan, IStringLocalizer localizer)
		{
			if(timeSpan.Hours > 0)
			{
				return $"{timeSpan:%h}{localizer["HoursShort"]} {timeSpan:%m}{localizer["MinutesShort"]}";
			}

			return $"{timeSpan:%m}{localizer["MinutesShort"]}";
		}
	}
}
