using PodcastClient.Data;
using System.Globalization;
using System.Xml.Linq;

namespace PodcastClient.Services
{
	public class PodcastRssFetcher
	{
		private static readonly CultureInfo _culture = CultureInfo.InvariantCulture;
		private static readonly string[] _dateFormats =
		[
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
		];

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

		private static Podcast? ParsePodcast(Uri rss, string? rssPage)
		{
			if (string.IsNullOrEmpty(rssPage))
			{
				return null;
			}

			var channel = XDocument.Parse(rssPage).Root!.Element("channel");
			List<Episode> episodes = [];
			HashSet<string> categories = [];
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

					case "category":
					{
						var category = element.Attribute("text")?.Value;
						if(category != null)
						{
							categories.Add(category);
						}

						break;
					}

					case "item":
					{
						try
						{
							var episode = ParseEpisode(element, iconSource);
							episodes.Add(episode);
						}
						catch { }

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
				Categories = categories.Select(c => new Category { Name = c }).ToList(),
				Episodes = episodes
			};
		}

		private static Episode ParseEpisode(XElement item, Uri podcastIconSource)
		{
			string title = "",
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
				Source = ParseUri(source),
				ContentSource = ParseUri(contentSource),
				IconSource = iconSource == "" ? podcastIconSource : ParseUri(iconSource),
				Type = ParseType(episodeType.Split('/').First())
			};
		}

		private static Uri ParseUri(string uri)
		{
			try
			{
				return new Uri(uri);
			}
			catch
			{
				throw new NotSupportedException($"Unable to parse episode uri: {uri}");
			}
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

			if(TimeSpan.TryParseExact(duration, "mm':'ss", _culture, out timeSpan))
			{
				return timeSpan;
			}

			throw new NotSupportedException($"Unable to parse episode duration: {duration}");
		}

		private static DateTime ParseDate(string releaseDate)
		{
			if (DateTime.TryParse(releaseDate, _culture, DateTimeStyles.None, out DateTime date))
			{
				return date.ToLocalTime();
			}

			if (DateTime.TryParseExact(releaseDate, _dateFormats, _culture,
									   DateTimeStyles.None, out date))
			{
				return date.ToLocalTime();
			}

			throw new NotSupportedException($"Unable to parse date: {releaseDate}");
		}

		private static EpisodeType ParseType(string type)
		{
			return type switch
			{
				"audio" => EpisodeType.Audio,
				"video" => EpisodeType.Video,
				_ => throw new NotSupportedException($"Unable to parse episode type: {type}")
			};
		}
	}
}
