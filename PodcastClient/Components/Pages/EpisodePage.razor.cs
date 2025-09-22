using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages
{
    public partial class EpisodePage
    {
		private bool _userHasEpisode =false;

		[Parameter]
		public int PodcastId { get; set; }

		[Parameter]
		public int EpisodeNumber { get; set; }

		public UserEpisode? EpisodePlayback { get; private set; }
		public Episode? Episode { get; private set; }
		public MarkupString Description { get; private set; }
		public string? PodcastTitle { get; private set; }

		protected override async Task OnInitializedAsync()
		{
			using var context = DbContextFactory.CreateDbContext();
			var query = await context.Episodes
				.AsNoTracking()
				.Where(e => e.EpisodeNumber == EpisodeNumber && e.PodcastId == PodcastId)
				.Select(e => new { Episode = e, PodcastTitle = e.Podcast.Title })
				.FirstOrDefaultAsync();
			Episode = query?.Episode;

			if(Episode != null)
			{
				EpisodePlayback = await context.UserEpisodes
					.AsNoTracking()
					.FirstOrDefaultAsync(ep => ep.EpisodeNumber == EpisodeNumber &&
											   ep.PodcastId == PodcastId &&
											   ep.UserId == UserContext.UserId);
				_userHasEpisode = EpisodePlayback != null;

				if (_userHasEpisode)
				{
					EpisodePlayback!.Episode = Episode;
				}
				else
				{
					EpisodePlayback = new UserEpisode { Episode = query!.Episode };
				}

				PodcastTitle = query!.PodcastTitle;
				Description = new(EpisodePlayback!.Episode.Description);
			}
		}

        private async Task OnPlay()
        {
			if (!_userHasEpisode)
			{
				return;
			}

            if(EpisodePlayback!.Status != EpisodeStatus.Played)
            {
                EpisodePlayback.Status = EpisodeStatus.InProgress;
				await UpdateEpisodeInDb();
			}
        }

		private async Task OnPause()
		{
			if (!_userHasEpisode)
			{
				return;
			}

			await UpdateEpisodeInDb();
		}

		private async Task OnEnded()
		{
			if (!_userHasEpisode)
			{
				return;
			}

			EpisodePlayback!.Status = EpisodeStatus.Played;
			await UpdateEpisodeInDb();
		}

		private async Task UpdateEpisodeInDb()
		{
			using var context = DbContextFactory.CreateDbContext();
			context.UserEpisodes.Update(EpisodePlayback!);
			await context.SaveChangesAsync();
		}
	}
}
