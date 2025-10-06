using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Services;
using static System.Formats.Asn1.AsnWriter;

namespace PodcastClient.Extensions
{
	public static class WebApplicationExtension
	{
		public static void SeedDatabase(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var services = scope.ServiceProvider;
			var configuration = app.Configuration;
			var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
			

			using var context = dbContextFactory.CreateDbContext();
			context.Database.EnsureCreated();

			AddPodcastCategories(context, services);
			AddDefaultPodcasts(context, services, configuration);
		}

		private static void AddPodcastCategories(ApplicationDbContext context,
												 IServiceProvider services)
		{
			if (context.Categories.Any())
			{
				return;
			}

			var categoriesService = services.GetRequiredService<CategoriesService>();
			var categoryNames = categoriesService.Categories
				.Select(c => new Category { Name = c.Name });

			context.Categories.AddRange(categoryNames);
			context.SaveChanges();
		}

		private static void AddDefaultPodcasts(ApplicationDbContext context,
											   IServiceProvider serivces,
											   IConfiguration configuration)
		{
			if (context.Podcasts.Any())
			{
				return;
			}

			var podcastRssFetcher = serivces.GetRequiredService<PodcastRssFetcher>();

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
