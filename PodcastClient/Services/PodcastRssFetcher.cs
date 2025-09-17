using PodcastClient.Models;
using System.Globalization;
using System.Xml.Linq;

namespace PodcastClient.Services
{
	public class PodcastRssFetcher
	{
		private static readonly string[] _dateFormats = {
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

		private readonly IHttpClientFactory _httpClientFactory;

		public PodcastRssFetcher(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<Podcast?> GetPodcastAsync(Uri rss)
		{
			var httpClient = _httpClientFactory.CreateClient();
			var rssPage = await httpClient.GetStringAsync(rss);
			return ParsePodcast(rss, rssPage);
		}

		public async Task<Podcast?> GetPodcastAsync(string? rss)
		{
			if(string.IsNullOrEmpty(rss))
			{
				return null;
			}

			try
			{
				var rssUri = new Uri(rss);
				return await GetPodcastAsync(rssUri);			
			}
			catch(Exception)
			{
				return null;
			}
		}

		private static Podcast? ParsePodcast(Uri rss, string? rssPage)
		{
			if (string.IsNullOrEmpty(rssPage))
			{
				return null;
			}

			var channel = XDocument.Parse(rssPage).Root!.Element("channel");
			List<Episode> episodes = [];
			Uri iconSource = Podcast.DefaultIconSource;
			string title = "",
				   author = "",
				   description = "";

			foreach (XElement element in channel!.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "title":
					{
						title = element.Value;
						break;
					}

					case "author":
					{
						author = element.Value;
						break;
					}

					case "description":
					{
						description = new(element.Value);
						break;
					}

					case "image":
					{
						if (element.Element("url") is XElement url)
						{
							iconSource = new(url.Value);
						}
						else if (element.Attribute("href") is XAttribute href)
						{
							iconSource = new(href.Value);
						}

						break;
					}

					case "item":
					{
						episodes.Add(ParseEpisode(element, iconSource));
						break;
					}
				}
			}

			for (int i = 0; i < episodes.Count; i++)
			{
				episodes[i].EpisodeNumber = episodes.Count - i;
			}

			return new Podcast
			{
				Title = title,
				Description = description,
				Rss = rss,
				Author = author,
				IconSource = iconSource,
				Episodes = episodes
			};
		}

		private static Episode ParseEpisode(XElement item, Uri podcastIconSource)
		{
			string id = "",
				   title = "",
				   description = "",
				   releaseDate = "",
				   duration = "",
				   source = "",
				   contentSource = "",
				   iconSource = "",
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
						source = element.Value;
						break;

					case "description":
						description = element.Value;
						break;

					case "pubDate":
						releaseDate = element.Value;
						break;

					case "enclosure":
						contentSource = element.Attribute("url")!.Value;
						episodeType = element.Attribute("type")!.Value;
						break;

					case "duration":
						duration = element.Value;
						break;

					case "image":
						iconSource = element.Attribute("href")!.Value;
						break;
				}
			}

			return new Episode()
			{
				Title = title,
				Description = description,
				ReleaseDate = ParseDate(releaseDate),
				Duration = ParseDuration(duration),
				Source = new Uri(source),
				ContentSource = new Uri(contentSource),
				IconSource = iconSource == "" ? podcastIconSource : new Uri(iconSource),
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

			if (DateTime.TryParseExact(releaseDate, _dateFormats,
									   CultureInfo.InvariantCulture,
									   DateTimeStyles.None, out date))
			{
				return date.ToLocalTime();
			}

			throw new NotSupportedException($"Unable to parse date: {releaseDate}");
		}
	}
}
