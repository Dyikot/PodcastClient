using Microsoft.EntityFrameworkCore;

namespace PodcastClient.Data
{
	public class Podcast
	{
		public static Uri DefaultIconSource { get; } = new Uri("Rss.svg", UriKind.Relative);

		public int Id { get; set; }

		public string Title { get; set; }
		public string Author { get; set; }
		public string Description { get; set; }
		public Uri Rss { get; set; }
		public int Subscribers { get; set; }
		public int EpisodesCount { get; set; }
		public DateOnly Inserted { get; set; }
		public DateTime LastUpdated { get; set; }
		public Uri IconSource { get; set; } = DefaultIconSource;

		public List<Category> Categories { get; set; } 
		public List<Episode> Episodes { get; set; }
		public List<User> Users { get; set; }

		public void UpdateInfo(PodcastUpdate podcastUpdate)
		{
			Title = podcastUpdate.Title;
			Author = podcastUpdate.Author;
			Description = podcastUpdate.Description;
			Rss = podcastUpdate.Rss;
			IconSource = podcastUpdate.IconSource;
			LastUpdated = podcastUpdate.LastUpdated;
		}

		public void SetEpisodes(List<Episode> episodes)
		{
			Episodes = episodes;
			EpisodesCount = episodes.Count;
		}

		public void SetCategories(IQueryable<Category> selection, List<string> categories)
		{
			Categories = selection
				.Where(c => categories.Contains(c.Name))
				.ToList();
		}

		public async Task SetCategoriesAsync(IQueryable<Category> selection, List<string> categories)
		{
			Categories = await selection
				.Where(c => categories.Contains(c.Name))
				.ToListAsync();
		}
	}

	public class PodcastUpdate
	{
		public required string Title { get; set; }
		public required string Author { get; set; }
		public required string Description { get; set; }
		public required Uri Rss { get; set; }
		public required Uri IconSource { get; set; }
		public required DateTime LastUpdated { get; set; }
		public required List<string> Categories { get; set; }
		public required List<Episode> NewEpisodes { get; set; }

		public Podcast ToPodcast(IQueryable<Category> selection)
		{
			Podcast podcast = new();
			podcast.UpdateInfo(this);
			podcast.SetCategories(selection, Categories);
			podcast.SetEpisodes(NewEpisodes);

			return podcast;
		}
	}
}
