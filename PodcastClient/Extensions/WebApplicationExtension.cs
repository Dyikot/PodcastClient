using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Services;
using System;
using System.Threading.Tasks;

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
			var categories = categoriesService.Categories
				.Select(c => new Category(c.Name));

			context.Categories.AddRange(categories);
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

			var podcastUpdates = podcastRssFetcher
				.GetMultipleUpdatesAsync(validPodcastUrls)
				.GetAwaiter()
				.GetResult();

			if (podcastUpdates.Count == 0)
			{
				return;
			}

			var podcasts = podcastUpdates
				.Select(podcastUpdate => podcastUpdate.ToPodcast(context.Categories))
				.ToArray();			

			context.Podcasts.AddRange(podcasts);
			context.SaveChanges();
		}
	}
}
