using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Services;

namespace PodcastClient.Extensions
{
	public static class WebApplicationExtension
	{
		public static void AddDefaultPodcasts(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
			var podcastRssFetcher = scope.ServiceProvider.GetRequiredService<PodcastRssFetcher>();

			using var context = dbContextFactory.CreateDbContext();
			context.Database.EnsureCreated();

			if (context.Podcasts.Any())
			{
				return;
			}

			var defaultPodcastUrls = app.Configuration
				.GetSection("DefaultPodcasts")
				.Get<string[]>();

			if(defaultPodcastUrls == null)
			{
				return;
			}

			var validPodcastUrls = defaultPodcastUrls
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

			context.Podcasts.AddRange(podcasts);
			context.SaveChanges();
		}
	}
}
