using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Services;

namespace PodcastClient.Extensions
{
	public static class WebApplicationExtension
	{
		public static void SeedDatabase(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
			var podcastRssFetcher = scope.ServiceProvider.GetRequiredService<PodcastRssFetcher>();

			using var context = dbContextFactory.CreateDbContext();
			context.Database.EnsureCreated();

			AddPodcastCategories(context, app.Configuration);
			AddDefaultPodcasts(context, app.Configuration, podcastRssFetcher);
		}

		private static void AddPodcastCategories(ApplicationDbContext context,
												 IConfiguration configuration)
		{
			if (context.Categories.Any())
			{
				return;
			}

			var podcastCategories = configuration
				.GetSection("PodcastCategories")
				.Get<string[]>()!
				.Select(categoryName => new Category { Name = categoryName })
				.ToArray();

			context.Categories.AddRange(podcastCategories);
			context.SaveChanges();
		}

		private static void AddDefaultPodcasts(ApplicationDbContext context, 
											   IConfiguration configuration,
											   PodcastRssFetcher podcastRssFetcher)
		{
			if (context.Podcasts.Any())
			{
				return;
			}

			var defaultPodcastUrls = configuration
				.GetSection("DefaultPodcasts")
				.Get<string[]>();

			var validPodcastUrls = defaultPodcastUrls!
				.Where(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
				.Select(url => new Uri(url))
				.ToArray();

			var podcastsTasks = validPodcastUrls
				.Select(podcastRssFetcher.GetPodcastAsync)
				.ToArray();

			if(podcastsTasks.Length == 0)
			{
				return;
			}

			var podcasts = Task.WhenAll(podcastsTasks).Result
				.Where(p => p != null)
				.Cast<Podcast>();

			foreach(var podcast in podcasts)
			{
				podcast.AttachCategories(context);
			}

			context.Podcasts.AddRange(podcasts);
			context.SaveChanges();
		}
	}
}
