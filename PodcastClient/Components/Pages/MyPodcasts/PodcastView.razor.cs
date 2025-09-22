using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class PodcastView
    {
        [Parameter, EditorRequired]
        public Podcast Podcast { get; set; }

        public int NewEpisodes { get; private set; }
        public string Href => $"podcast/{Podcast.Id}";

		protected override async Task OnInitializedAsync()
		{
            using var context = DbContextFactory.CreateDbContext();

			NewEpisodes = await context.UserEpisodes
                    .AsNoTracking()
					.Where(ep => ep.UserId == UserContext.UserId &&
                                 ep.PodcastId == Podcast.Id)
					.CountAsync(ep => ep.Status != EpisodeStatus.Played);
		}
    }
}
