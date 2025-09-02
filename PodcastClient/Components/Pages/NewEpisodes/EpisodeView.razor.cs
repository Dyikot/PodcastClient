using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class EpisodeView
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }

		private void OnEpisodeCLick()
        {
            PodcastCollection.Current = Episode;
            Navigator.NavigateTo("/now-playing");
        }
    }
}
