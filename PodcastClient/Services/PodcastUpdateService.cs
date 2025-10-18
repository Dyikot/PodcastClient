using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Services
{
	public class PodcastUpdateService : BackgroundService
	{
		private readonly ILogger<PodcastUpdateService> _logger;
		private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
		private readonly PodcastRssFetcher _podcastRssFetcher;

		public PodcastUpdateService(ILogger<PodcastUpdateService> logger,
									IDbContextFactory<ApplicationDbContext> dbContextFactory,
									PodcastRssFetcher podcastRssFetcher)
		{
			_logger = logger;
			_dbContextFactory = dbContextFactory;
			_podcastRssFetcher = podcastRssFetcher;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Delay(TimeSpan.FromDays(1), stoppingToken);

			while (!stoppingToken.IsCancellationRequested)
			{
				await UpdatePodcastsAsync();				
				await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
			}
		}

		private async Task UpdatePodcastsAsync()
		{
			_logger.LogInformation("Starting podcasts update cycle");
			using var context = _dbContextFactory.CreateDbContext();
			const int take = 10;
			DateTime yesterday = DateTime.Now.AddDays(-1);
			List<Podcast> podcasts;

			for (int skip = 0; ; skip += take)
			{
				podcasts = await context.Podcasts
					.Skip(skip)
					.Take(take)
					.ToListAsync();				

				if (podcasts.Count == 0)
				{
					break;
				}

				var uris = podcasts.Select(p => p.Rss);
				var podcastsDictionary = podcasts.ToDictionary(p => p.Title, p => p);
				var podcastUpdates = await _podcastRssFetcher
					.GetMultipleUpdatesAsync(uris, yesterday);
				
				foreach(var podcastUpdate in podcastUpdates)
				{
					var podcast = podcastsDictionary[podcastUpdate.Title];
					await ApplyPodcastUpdate(context, podcastUpdate, podcast);
				}

				await context.SaveChangesAsync();
			}

			_logger.LogInformation("Podcasts update completed successfully");
		}

		private static async Task ApplyPodcastUpdate(ApplicationDbContext context, 
													 PodcastUpdate podcastUpdate, 
													 Podcast podcast)
		{
			podcast.UpdateInfo(podcastUpdate);
			podcast.SetCategories(context.Categories, podcastUpdate.Categories);
			podcast.EpisodesCount += podcastUpdate.NewEpisodes.Count;

			int episodeNumber = podcast.EpisodesCount - 1;
			foreach (var episode in podcastUpdate.NewEpisodes)
			{
				episode.PodcastId = podcast.Id;
				episode.EpisodeNumber = episodeNumber--;
			}

			await context.Episodes.AddRangeAsync(podcastUpdate.NewEpisodes);
		}
	}
}
