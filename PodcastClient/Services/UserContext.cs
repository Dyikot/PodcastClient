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

		public async Task InitializeAsync()
		{
			var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var claim = state.User.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				UserId = int.Parse(claim.Value);
			}
		}
	}
}
