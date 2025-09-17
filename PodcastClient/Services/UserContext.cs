using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Services
{
	public class UserContext
	{
		private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;		

		public UserContext(IDbContextFactory<ApplicationDbContext> dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
			using var context = _dbContextFactory.CreateDbContext();

			if (!context.Users.Any())
			{
				context.Users.Add(new User());
				context.SaveChanges();
			}

			UserId = context.Users.First().Id;
		}

		public int UserId { get; init; }

		public async Task AddPodcastAsync(Podcast podcast)
		{
			using var context = _dbContextFactory.CreateDbContext();
			
			var user = await context.Users
				.Include(u => u.Podcasts)
				.FirstAsync(u => u.Id == UserId);

			var userEpisodes = podcast.Episodes
				.Select(ep => new UserEpisode { User = user, Episode = ep })
				.ToList();

			user.Podcasts.Add(podcast);
			context.UserEpisodes.AddRange(userEpisodes);

			await context.SaveChangesAsync();
		}

		public async Task RemovePodcastAsync(int podcastId)
		{
			using var context = _dbContextFactory.CreateDbContext();

			var sql = "DELETE FROM PodcastUser WHERE PodcastsId = {0} AND UsersId = {1}";
			await context.Database.ExecuteSqlRawAsync(sql, podcastId, UserId);

			await context.UserEpisodes
				.Where(ep => ep.UserId == UserId && ep.PodcastId == podcastId)
				.ExecuteDeleteAsync();
		}
	}
}
