using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class NewEpisodes
    {
        public List<Episode> Episodes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            using var context = DbContextFactory.CreateDbContext();
			Episodes = await context.UserEpisodes
                .AsNoTracking()
                .Where(ep => ep.UserId == UserContext.UserId &&
                             ep.Status != EpisodeStatus.Played)
                .Include(ep => ep.Episode)
                .Select(ep => ep.Episode)
                .OrderByDescending(e => e.ReleaseDate)
                .ToListAsync();
        }
    }
}
