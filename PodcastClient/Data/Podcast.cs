using Microsoft.EntityFrameworkCore;

namespace PodcastClient.Data
{
	public class Podcast
	{
		public static Uri DefaultIconSource { get; } = new Uri("Rss.svg", UriKind.Relative);

		public int Id { get; set; }

		public required string Title { get; set; }
		public required string Author { get; set; }
		public required string Description { get; set; }
		public required Uri Rss { get; set; }
		public int Subscribers { get; set; }
		public DateOnly Inserted { get; set; }
		public Uri IconSource { get; set; } = DefaultIconSource;

		public List<Category> Categories { get; set; } 
		public List<Episode> Episodes { get; set; }
		public List<User> Users { get; set; }

		public void AttachCategories(ApplicationDbContext context)
		{
			var categoryNames = Categories.Select(c => c.Name).ToList();
			Categories = context.Categories
				.Where(c => categoryNames.Contains(c.Name))
				.ToList();			
		}

		public async Task AttachCategoriesAsync(ApplicationDbContext context)
		{
			var categoryNames = Categories.Select(c => c.Name).ToList();
			Categories = await context.Categories
				.Where(c => categoryNames.Contains(c.Name))
				.ToListAsync();
		}
	}
}
