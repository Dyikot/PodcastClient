using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NewEpisodes
{
    public partial class EpisodeView
    {
		[Parameter, EditorRequired]
		public EpisodeViewModel DataContext { get; set; }

        public Episode Episode => DataContext.Episode;
		public string PodcastTitle => DataContext.PodcastTitle;

		private void OnEpisodeClick()
        {
            var podcastId = Episode.PodcastId;
            var episodeNumber = Episode.EpisodeNumber;

            Navigator.NavigateTo($"/podcast/{podcastId}/episode/{episodeNumber}");
        }
    }
}
