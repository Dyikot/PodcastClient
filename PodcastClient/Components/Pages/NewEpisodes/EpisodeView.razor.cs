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
        public string Href => $"/podcast/{Episode.PodcastId}/episode/{Episode.EpisodeNumber}";
    }
}
