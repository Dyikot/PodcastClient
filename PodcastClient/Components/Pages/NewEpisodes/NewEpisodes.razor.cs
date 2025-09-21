using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public readonly struct EpisodeViewModel
    {
        public Episode Episode { get; init; }
        public string PodcastTitle { get; init; }
    }

    public partial class NewEpisodes
    {
        public List<EpisodeViewModel> Episodes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            using var context = DbContextFactory.CreateDbContext();
			Episodes = await context.UserEpisodes
                .AsNoTracking()
                .Where(ep => ep.UserId == UserContext.UserId &&
                             ep.Status != EpisodeStatus.Played)
                .Include(ep => ep.Episode)
                .Select(ep => new EpisodeViewModel 
                    {
                        Episode = ep.Episode,
                        PodcastTitle = ep.Episode.Podcast.Title 
                    })
                .OrderByDescending(vm => vm.Episode.ReleaseDate)
                .ToListAsync();
        }
    }
}
