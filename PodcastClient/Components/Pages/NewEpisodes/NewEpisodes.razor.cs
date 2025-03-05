using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class NewEpisodes
    {
        public IList<Episode> Episodes { get; set; } = [];

        protected override void OnInitialized()
        {
            Episodes = PodcastService.Podcasts
                .SelectMany(podcast => podcast.NewEpisodes)
                .OrderByDescending(episode => episode.ReleaseDate)
                .ToList();

            base.OnInitialized();
        }
    }
}
