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
		private readonly ILogger<PodcastRssFetcher> _logger;

		public PodcastRssFetcher(IHttpClientFactory httpClientFactory,
								 ILogger<PodcastRssFetcher> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<PodcastUpdate?> GetUpdateAsync(Uri rss)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient();
				var rssPage = await httpClient.GetStringAsync(rss);
				if (string.IsNullOrEmpty(rssPage))
				{
					return null;
				}

				return ParsePodcastUpdate(rss, rssPage);
			}
			catch(Exception)
			{
				_logger.LogError("Enable to get rss page: '{}'", rss);
				return null;
			}
		}

		public async Task<PodcastUpdate?> GetUpdateAsync(Uri rss, DateTime updateTime)
		{
			try
			{
				var httpClient = _httpClientFactory.CreateClient();
				var rssPage = await httpClient.GetStringAsync(rss);

				if (string.IsNullOrEmpty(rssPage))
				{
					return null;
				}

				return ParsePodcastUpdate(rss, rssPage, updateTime);
			}
			catch(Exception)
			{
				_logger.LogError("Enable to get rss page: '{}'", rss);
				return null;
			}
		}

		public async Task<List<PodcastUpdate>> GetMultipleUpdatesAsync(IEnumerable<Uri> rssUris)
		{
			if (!rssUris.Any())
			{
				return [];
			}

			var tasks = rssUris.Select(GetUpdateAsync);
			var results = await Task.WhenAll(tasks);

			return results.Where(result => result != null).ToList()!;
		}

		public async Task<List<PodcastUpdate>> GetMultipleUpdatesAsync(IEnumerable<Uri> rssUris, 
																	   DateTime updateTime)
		{
			if (!rssUris.Any())
			{
				return [];
			}

			var tasks = rssUris.Select(uri => GetUpdateAsync(uri, updateTime));
			var results = await Task.WhenAll(tasks);

			return results.Where(result => result != null).ToList()!;
		}

		private PodcastUpdate ParsePodcastUpdate(Uri rss, string rssPage)
		{
			var channel = XDocument.Parse(rssPage).Root!.Element("channel");

			string title = "";
			string author = "";
			string description = "";
			DateTime lastUpdated = DateTime.Now;
			Uri iconSource = Podcast.DefaultIconSource;
			List<Episode> episodes = [];
			HashSet<string> categories = [];			

			foreach (var element in channel!.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "title":
						title = element.Value;
						break;

					case "author":
						author = element.Value;
						break;

					case "description":
						description = element.Value;
						break;

					case "lastBuildDate":
						lastUpdated = ParseDate(element.Value);
						break;

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
						if (element.Attribute("text")?.Value is string category)
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
						catch (Exception ex)
						{
							_logger.LogError("Error occur while parsing '{}' podcast episode: {}", title, ex.Message);
						}

						break;
					}
				}
			}

			int episodeNumber = episodes.Count - 1;
			foreach(var episode in episodes)
			{
				episode.EpisodeNumber = episodeNumber--;
			}

			return new PodcastUpdate
			{
				Title = title,
				Description = description,
				Rss = rss,
				Author = author,
				LastUpdated = lastUpdated,
				IconSource = iconSource,
				Categories = categories.ToList(),
				NewEpisodes = episodes
			};
		}

		private PodcastUpdate ParsePodcastUpdate(Uri rss, string rssPage, DateTime updateTime)
		{
			var channel = XDocument.Parse(rssPage).Root!.Element("channel");

			string title = "";
			string author = "";
			string description = "";
			DateTime lastPodcastUpdated = DateTime.Now;
			Uri iconSource = Podcast.DefaultIconSource;
			List<Episode> episodes = [];
			HashSet<string> categories = [];

			bool shouldBreak = false;

			foreach (var element in channel!.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "title":
						title = element.Value;
						break;

					case "author":
						author = element.Value;
						break;

					case "description":
						description = element.Value;
						break;

					case "lastBuildDate":
						lastPodcastUpdated = ParseDate(element.Value);
						shouldBreak = lastPodcastUpdated < updateTime;
						break;

					case "image":
						if (element.Element("url") is XElement url)
						{
							iconSource = new(url.Value);
						}
						else if (element.Attribute("href") is XAttribute href)
						{
							iconSource = new(href.Value);
						}
						break;

					case "category":
						if (element.Attribute("text")?.Value is string category)
						{
							categories.Add(category);
						}

						break;

					case "item":
						try
						{
							var episode = ParseEpisode(element, iconSource);
							shouldBreak = episode.ReleaseDate < updateTime;

							if (!shouldBreak)
							{
								episodes.Add(episode);
							}
						}
						catch(Exception ex) 
						{
							_logger.LogError("Error occur while parsing '{}' podcast episode: {}", title, ex.Message);
						}

						break;
				}

				if (shouldBreak)
				{
					break;
				}
			}

			return new PodcastUpdate
			{
				Title = title,
				Author = author,
				Description = description,
				Rss = rss,
				LastUpdated = lastPodcastUpdated,
				IconSource = iconSource,
				Categories = categories.ToList(),
				NewEpisodes = episodes,
			};
		}

		private static Episode ParseEpisode(XElement item, Uri podcastIconSource)
		{
			string title = "";
			string description = "";
			string releaseDate = "";
			string duration = "";
			string contentSource = "";
			string iconSource = "";
			string episodeType = "";

			foreach (var element in item.Elements())
			{
				switch (element.Name.LocalName)
				{
					case "title":
						title = element.Value;
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
				ContentSource = ParseUri(contentSource),
				IconSource = iconSource == "" ? podcastIconSource : ParseUri(iconSource),
				Type = ParseType(episodeType.Split('/').First())
			};
		}

		private static Uri ParseUri(string uri)
		{
			if(Uri.TryCreate(uri, UriKind.Absolute, out var result))
			{
				return result;
			}

			throw new NotSupportedException($"Unable to parse episode uri: '{uri}'");
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

			throw new NotSupportedException($"Unable to parse episode duration: '{duration}'");
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

			throw new NotSupportedException($"Unable to parse date: '{releaseDate}'");
		}

		private static EpisodeType ParseType(string type)
		{
			return type switch
			{
				"audio" => EpisodeType.Audio,
				"video" => EpisodeType.Video,
				_ => throw new NotSupportedException($"Unable to parse episode type: '{type}'")
			};
		}
	}
}
