using Microsoft.Extensions.Localization;

namespace PodcastClient.Extensions
{
	public static class DateTimeExtension
	{
		public static string ToLocalizedString(this DateTime date, IStringLocalizer localizer)
		{
			var period = DateTime.Now - date;

			if (period < TimeSpan.FromHours(1))
			{
				return $"{period:%m}{localizer["MinutesShort"]} {localizer["ago"]}";
			}

			if (period < TimeSpan.FromDays(1))
			{
				return $"{period:%h}{localizer["HoursShort"]} {localizer["ago"]}";
			}

			if (period < TimeSpan.FromDays(7))
			{
				return $"{period:%d}{localizer["DaysShort"]} {localizer["ago"]}";
			}

			if (DateTime.Now.Year == date.Year)
			{
				return $"{date:M}";
			}

			return $"{date:M}, {date:yyyy}";
		}
	}
}
