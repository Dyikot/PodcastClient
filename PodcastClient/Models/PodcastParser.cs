using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Xml.Linq;

namespace PodcastClient.Models
{
    public partial class Podcast
    {
        private static readonly string[] _formats = [
            "ddd, dd MMM yyyy HH:mm:ss 'PDT'",
            "ddd, dd MMM yyyy HH:mm:ss PDT"
        ];

		public static Podcast? TryParse(string rss)
        {
            if (rss == string.Empty)
            {
                return null;
            }

            XElement? channel = XDocument.Parse(rss).Root?.Element("channel");

            string title = string.Empty,
                   author = string.Empty,
                   description = string.Empty,
                   iconUrl = string.Empty;
            List<Episode> episodes = [];

            foreach (XElement element in channel!.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "title": title = element.Value; break;
                    case "author": author = element.Value; break;
                    case "description": description = element.Value; break;
                    case "image":
                        if (iconUrl != string.Empty)
                        {
                            break;
                        }

                        if (element.Element("url") is XElement url)
                        {
                            iconUrl = url.Value;
                        }

                        if (element.Attribute("href") is XAttribute href)
                        {
                            iconUrl = href.Value;
                        }

                        break;
                    case "item":
                        episodes.Add(ParseRssEpisode(element));
                        break;
                }
            }

            for (int i = 0; i < episodes.Count; i++)
            {
                episodes[i].Number = episodes.Count - i;
            }

            return new Podcast()
            {
                Name = title,
                Author = author,
                Description = new MarkupString(description),
                IconUrl = new Uri(iconUrl),
                Episodes = episodes.Cast<Episode>().ToList()
			};
        }

        private static Episode ParseRssEpisode(XElement item)
        {
            string id = string.Empty,
                   title = string.Empty,
                   description = string.Empty,
                   releaseDate = string.Empty,
                   duration = string.Empty,
                   link = string.Empty,
                   contentUri = string.Empty,
                   iconUri = string.Empty,
                   episodeType = string.Empty;

            foreach (var element in item.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "guid": id = element.Value; break;
                    case "title": title = element.Value; break;
                    case "link": link = element.Value; break;
                    case "description": description = element.Value; break;
                    case "pubDate": releaseDate = element.Value; break;
                    case "enclosure":
                        contentUri = element.Attribute("url")!.Value;
                        episodeType = element.Attribute("type")!.Value;
                        break;
                    case "duration": duration = element.Value; break;
                    case "image": iconUri = element.Attribute("href")!.Value; break;
                }
            }

            return new Episode()
            {
                Id = Guid.TryParse(id, out Guid guid) ? guid : Guid.NewGuid(),
                Name = title,
                Description = new MarkupString(description),
                ReleaseDate = ParseReleaseDate(releaseDate),
                Duration = ParseDuration(duration),
                Link = new Uri(link),
                ContentUri = new Uri(contentUri),
                IconUri = iconUri == string.Empty ? null : new Uri(iconUri),
                Type = episodeType.Split('/').First() switch { 
                    "audio" => ContentType.Audio,
                    "video" => ContentType.Video,
                    _ => throw new  NotSupportedException()
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

            throw new NotSupportedException("Enable to parse episode duration!");
        }

        private static DateTime ParseReleaseDate(string releaseDate)
        {
			if (DateTime.TryParse(releaseDate, out DateTime date))
            {
                return date.ToLocalTime();
            }
            else if(DateTime.TryParseExact(releaseDate, _formats, 
                                           CultureInfo.InvariantCulture,
				                           DateTimeStyles.None, out date))
            {
                return date.ToLocalTime();
            }

			throw new NotSupportedException("Enable to parse date!");
		}
    }
}
