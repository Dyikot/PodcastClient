using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Xml.Linq;

namespace PodcastClient.Models
{
    public partial class Podcast
    {
		public static Podcast? TryParse(string rss)
        {
            if (rss == string.Empty)
            {
                return null;
            }

            XElement? channel = XDocument.Parse(rss).Root?.Element("channel");

            string title = "",
                   author = "",
                   description = "",
                   iconUrl = "";
            List<Episode> episodes = [];

            foreach (XElement element in channel!.Elements())
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

                    case "image":
                        if (iconUrl != string.Empty)
                        {
                            break;
                        }

                        if (element.Element("url") is XElement url)
                        {
                            iconUrl = url.Value;
                        }
                        else if (element.Attribute("href") is XAttribute href)
                        {
                            iconUrl = href.Value;
                        }

                        break;

                    case "item":
                        episodes.Add(Episode.Parse(element));
                        break;
                }
            }

            for (int i = 0; i < episodes.Count; i++)
            {
                episodes[i].Number = episodes.Count - i;
            }

            return new Podcast(episodes)
            {
                Name = title,
                Author = author,
                Description = new MarkupString(description),
                IconUrl = new Uri(iconUrl)
			};
        }        
    }
}
