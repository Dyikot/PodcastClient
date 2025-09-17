using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class PodcastView
    {
        [Parameter, EditorRequired]
        public Podcast Podcast { get; set; }
        [Parameter]
        public EventCallback<Podcast> Click { get; set; }

        public int NewEpisodes { get; private set; }

		protected override async Task OnInitializedAsync()
		{
            using var context = DbContextFactory.CreateDbContext();

			NewEpisodes = await context.UserEpisodes
                    .AsNoTracking()
					.Where(ep => ep.UserId == UserContext.UserId &&
                                 ep.PodcastId == Podcast.Id)
					.CountAsync(ep => ep.Status != EpisodeStatus.Played);
		}

        private Task OnClick() => Click.InvokeAsync(Podcast);
    }
}
