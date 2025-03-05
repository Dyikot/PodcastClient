using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class EpisodeBlock
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }
        public string Duration => Episode is Episode episode
                                  ? episode.Duration.ToString("hh':'mm")
                                  : string.Empty;

        private void OnEpisodeCLick()
        {
            PodcastService.Playing = Episode;
            Navigator.NavigateTo("/now-playing");
        }
    }
}
