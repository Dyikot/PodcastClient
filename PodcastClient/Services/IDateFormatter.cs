using Microsoft.Extensions.Localization;

namespace PodcastClient.Services
{
    public interface IDateFormatter
    {
        public string ToFormattedString(DateTime date, IStringLocalizer localizer);
    }

    public class DateFormatter : IDateFormatter
    {
        public string ToFormattedString(DateTime date, IStringLocalizer localizer)
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
