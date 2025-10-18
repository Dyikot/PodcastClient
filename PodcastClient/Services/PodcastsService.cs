using Microsoft.EntityFrameworkCore;
using PodcastClient.Base;
using PodcastClient.Data;
using System.Linq.Expressions;

namespace PodcastClient.Services
{
	public class PodcastsService
	{
		private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
		private readonly PodcastRssFetcher _podcastRssFetcher;

		public PodcastsService(IDbContextFactory<ApplicationDbContext> dbContextFactory, 
							   PodcastRssFetcher podcastRssFetcher)
		{
			_dbContextFactory = dbContextFactory;
			_podcastRssFetcher = podcastRssFetcher;
		}

		public async Task<Podcast?> FindAsync(Expression<Func<Podcast, bool>> predicate)
		{
			using var context = _dbContextFactory.CreateDbContext();

			return await context.Podcasts
				.AsNoTracking()
				.Include(p => p.Episodes)
				.FirstOrDefaultAsync(predicate);
		}

		public async Task<Podcast?> FindAsync(int podcastId) => 
			await FindAsync(p => p.Id == podcastId);

		public async Task<List<Podcast>> SearchAsync(string title, int skip, int take)
		{
			using var context = _dbContextFactory.CreateDbContext();

			return await context.Podcasts
				.AsNoTracking()
				.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
				.Skip(skip)
				.Take(take)
				.ToListAsync();
		}

		public async Task AddAsync(Podcast podcast)
		{
			using var context = _dbContextFactory.CreateDbContext();

			await context.AddAsync(podcast);
			await context.SaveChangesAsync();
		}

		public async Task<Podcast?> TryAddAsync(Uri rss)
		{
			Podcast? podcast = null;

			var podcastUpdate = await _podcastRssFetcher.GetUpdateAsync(rss);
			if (podcastUpdate != null)
			{
				using var context = _dbContextFactory.CreateDbContext();

				podcast = podcastUpdate.ToPodcast(context.Categories);
				await context.AddAsync(podcast);
				await context.SaveChangesAsync();
			}

			return podcast;
		}

		public async Task RemoveAsync(int podcastId)
		{
			using var context = _dbContextFactory.CreateDbContext();
			await context.Podcasts.Where(p => p.Id == podcastId).ExecuteDeleteAsync();
		}
	}
}
