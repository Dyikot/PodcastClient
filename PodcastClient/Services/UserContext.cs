using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using System.Security.Claims;

namespace PodcastClient.Services
{
	public class UserContext
	{
		private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
		private readonly AuthenticationStateProvider _authenticationStateProvider;

		public UserContext(IDbContextFactory<ApplicationDbContext> dbContextFactory,
						   AuthenticationStateProvider authenticationStateProvider)
		{
			_dbContextFactory = dbContextFactory;
			_authenticationStateProvider = authenticationStateProvider;
		}

		public int UserId { get; private set; }
		public bool IsAuthenticated => UserId > 0;

		public async Task AddPodcastAsync(Podcast podcast)
		{
			using var context = _dbContextFactory.CreateDbContext();
			
			var user = await context.Users
				.Include(u => u.Podcasts)
				.FirstAsync(u => u.Id == UserId);

			context.Podcasts.Attach(podcast);

			var userEpisodes = podcast.Episodes
				.Select(ep => new UserEpisode { User = user, Episode = ep })
				.ToList();

			user.Podcasts.Add(podcast);
			context.UserEpisodes.AddRange(userEpisodes);
			podcast.Subscribers++;

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

			await context.Podcasts
				.Where(p => p.Id == podcastId)
				.ExecuteUpdateAsync(s => s.SetProperty(p => p.Subscribers, p => p.Subscribers - 1));
		}

		public async Task InitializeAsync()
		{
			var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var claim = state.User.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				UserId = int.Parse(claim.Value);
				await UpdatePodcastsAsync();
			}
		}

		private async Task UpdatePodcastsAsync()
		{
			using var context = _dbContextFactory.CreateDbContext();

			var user = await context.Users.FindAsync(UserId);
			var lastUpdateChecked = user!.LastUpdateChecked;

			if (DateTime.Now - lastUpdateChecked < TimeSpan.FromDays(1))
			{
				return;
			}

			var newEpisodes = await context.Users
				.AsNoTracking()
				.Where(u => u.Id == UserId)
				.SelectMany(u => u.Podcasts)
				.Where(p => p.LastUpdated > lastUpdateChecked)
				.SelectMany(p => p.Episodes)
				.Where(e => e.ReleaseDate > lastUpdateChecked)
				.ToListAsync();
				
			var newUserEpisodes = newEpisodes
				.Select(e => new UserEpisode
				{
					UserId = UserId,
					PodcastId = e.PodcastId,
					EpisodeNumber = e.EpisodeNumber
				})
				.ToList();

			user.LastUpdateChecked = DateTime.Now;

			await context.UserEpisodes.AddRangeAsync(newUserEpisodes);
			await context.SaveChangesAsync();
		}
	}
}
