using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class EpisodeView
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }

		private void OnEpisodeClick()
        {
            var podcastId = Episode.PodcastId;
            var episodeNumber = Episode.EpisodeNumber;

            Navigator.NavigateTo($"/podcast/{podcastId}/episode/{episodeNumber}");
        }
    }
}
