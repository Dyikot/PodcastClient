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
            PodcastCollection.Current = Episode;
            Navigator.NavigateTo("/now-playing");
        }
    }
}
