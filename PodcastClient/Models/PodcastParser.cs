using Microsoft.AspNetCore.Components;
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

            var channel = XDocument.Parse(rss).Root!.Element("channel");
            var podcast = new Podcast();
            bool hasIconInitialized = false;

            foreach (XElement element in channel!.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "title":
                    {
						podcast.Title = element.Value;
						break;
					}

                    case "author":
                    {
						podcast.Author = element.Value;
						break;
					}

                    case "description":
                    {
						podcast.Description = new(element.Value);
						break;
					}

                    case "image":
                    {
						if (hasIconInitialized)
						{
							break;
						}

						if (element.Element("url") is XElement url)
						{
							podcast.IconUrl = new(url.Value);
                            hasIconInitialized = true;
						}
						else if (element.Attribute("href") is XAttribute href)
						{
							podcast.IconUrl = new(href.Value);
                            hasIconInitialized = true;
						}

						break;
					}

                    case "item":
                    {
						podcast.Episodes.Add(Episode.Parse(element));
						break;
					}
                }
            }

            for (int i = 0; i < podcast.Episodes.Count; i++)
            {
                podcast.Episodes[i].Number = podcast.Episodes.Count - i;
            }

            return podcast;
        }        
    }
}
