using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class NewEpisodes
    {
        private const int ItemsPerPage = 25;

        [SupplyParameterFromQuery]
        public int Page { get; set; }

        public int PageAmount => (Episodes.Count + ItemsPerPage - 1) / ItemsPerPage;

        public List<EpisodeViewModel> Episodes { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            using var context = DbContextFactory.CreateDbContext();
			Episodes = await context.UserEpisodes
                .AsNoTracking()
                .Where(ep => ep.UserId == UserContext.UserId &&
                             ep.Status != EpisodeStatus.Played)
                .OrderByDescending(ep => ep.Episode.ReleaseDate)
                .Select(ep => new EpisodeViewModel 
                    {
                        Episode = ep.Episode,
                        PodcastTitle = ep.Episode.Podcast.Title 
                    })
                .ToListAsync();

            if(Page < 0 || Page > PageAmount)
            {
                throw new NotSupportedException();
			}
            else if(Page == 0)
            {
                Page = 1;
            }
        }
    }
}
