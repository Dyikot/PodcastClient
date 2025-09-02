using Microsoft.AspNetCore.Components;

namespace PodcastClient.Models
{
	public partial class Podcast
	{
		private static Uri DefaultIconUri { get; } = new Uri("Rss.svg", UriKind.Relative);

		public Podcast()
		{
			Episodes = new EpisodeCollection(this);
		}

		public string Title { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public MarkupString Description { get; set; } = new();
		public Uri IconUrl { get; set; } = DefaultIconUri;
		public EpisodeCollection Episodes { get; init; }
	}
}
