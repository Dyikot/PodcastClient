using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Xml.Linq;

namespace PodcastClient.Models
{
	public partial class Episode
	{
		private static readonly string[] _formats = {
			"ddd, dd MMM yyyy HH:mm:ss 'GMT'",
			"ddd, dd MMM yyyy HH:mm:ss GMT",
			"ddd, dd MMM yyyy HH:mm:ss 'UTC'",
			"ddd, dd MMM yyyy HH:mm:ss UTC",
			"ddd, dd MMM yyyy HH:mm:ss 'PDT'",
			"ddd, dd MMM yyyy HH:mm:ss PDT",
			"ddd, dd MMM yyyy HH:mm:ss 'EST'",
			"ddd, dd MMM yyyy HH:mm:ss EST",
			"ddd, dd MMM yyyy HH:mm:ss 'EDT'",
			"ddd, dd MMM yyyy HH:mm:ss EDT",
			"ddd, dd MMM yyyy HH:mm:ss 'CST'",
			"ddd, dd MMM yyyy HH:mm:ss CST",
		};

		public static Episode Parse(XElement item)
		{
			string id = "",
				   title = "",
				   description = "",
				   releaseDate = "",
				   duration = "",
				   link = "",
				   contentUri = "",
				   iconUri = "",
				   episodeType = "";

			foreach (var element in item.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "guid":
						id = element.Value;
						break;

					case "title":
						title = element.Value;
						break;

					case "link":
						link = element.Value;
						break;

					case "description":
						description = element.Value;
						break;

					case "pubDate":
						releaseDate = element.Value;
						break;

					case "enclosure":
						contentUri = element.Attribute("url")!.Value;
						episodeType = element.Attribute("type")!.Value;
						break;

					case "duration":
						duration = element.Value;
						break;

					case "image":
						iconUri = element.Attribute("href")!.Value;
						break;
				}
			}

			return new Episode()
			{
				Title = title,
				Description = new MarkupString(description),
				ReleaseDate = ParseDate(releaseDate),
				Duration = ParseDuration(duration),
				Link = new Uri(link),
				ContentUri = new Uri(contentUri),
				IconUri = iconUri == string.Empty ? null : new Uri(iconUri),
				Type = episodeType.Split('/').First() switch
				{
					"audio" => EpisodeType.Audio,
					"video" => EpisodeType.Video,
					_ => throw new NotSupportedException()
				}
			};
		}

		private static TimeSpan ParseDuration(string duration)
		{
			if (double.TryParse(duration, out double seconds))
			{
				return TimeSpan.FromSeconds(seconds);
			}

			if (TimeSpan.TryParse(duration, out TimeSpan timeSpan))
			{
				return timeSpan;
			}

			throw new NotSupportedException($"Unable to parse episode duration: {duration}");
		}

		private static DateTime ParseDate(string releaseDate)
		{
			if (DateTime.TryParse(releaseDate, CultureInfo.InvariantCulture,
								  DateTimeStyles.None, out DateTime date))
			{
				return date.ToLocalTime();
			}

			if (DateTime.TryParseExact(releaseDate, _formats,
									   CultureInfo.InvariantCulture,
									   DateTimeStyles.None, out date))
			{
				return date.ToLocalTime();
			}

			throw new NotSupportedException($"Unable to parse date: {releaseDate}");
		}
	}
}
