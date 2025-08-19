using Microsoft.AspNetCore.Components;

namespace PodcastClient.Models
{
	public partial class Podcast
	{
		public Podcast()
		{
			Episodes = new EpisodeCollection(this);
		}

		public Podcast(IList<Episode> episodes)
		{
			Episodes = new EpisodeCollection(this, episodes);
		}

		public required string Name { get; set; }
		public required string Author { get; set; }
		public required MarkupString Description { get; set; }
		public Uri IconUrl { get; set; } = new Uri("Rss.svg", UriKind.Relative);

		public EpisodeCollection Episodes { get; private set; }
	}
}
