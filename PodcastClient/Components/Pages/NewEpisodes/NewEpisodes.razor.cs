using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class NewEpisodes
    {
        public IList<Episode> Episodes { get; set; } = [];

        protected override void OnInitialized()
        {
            Episodes = PodcastCollection
                .SelectMany(podcast => podcast.Episodes.Where(ep => ep.Status != EpisodeStatus.Played))
                .OrderByDescending(episode => episode.ReleaseDate)
                .ToList();

            base.OnInitialized();
        }
    }
}
