using Microsoft.AspNetCore.Components;

namespace PodcastClient.Models
{
	public partial class Podcast
	{
		private EpisodesList _items;

		public Podcast() => _items = new EpisodesList(this);

		public Guid Id { get; set; } = Guid.NewGuid();
		public required string Name { get; set; }
		public required string Author { get; set; }
		public required MarkupString Description { get; set; }
		public Uri IconUrl { get; set; } = new Uri("Rss.svg", UriKind.Relative);

		public IList<Episode> Episodes { get => _items; set => _items = new EpisodesList(value, this); }
		public IList<Episode> NewEpisodes => Episodes.Where(item => item.Status != PlayStatus.Played).ToList();
		public IList<Episode> PlayedEpisodes => Episodes.Where(item => item.Status == PlayStatus.Played).ToList();
		public IList<Episode> UnplayedEpisodes => Episodes.Where(item => item.Status == PlayStatus.Unplayed).ToList();
		public IList<Episode> InProgressEpisodes => Episodes.Where(item => item.Status == PlayStatus.InProgress).ToList();
	}
}
