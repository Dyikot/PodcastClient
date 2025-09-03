using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public partial class EpisodeView
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }

		private void OnNotStartedMarkClick() => Episode.Status = EpisodeStatus.Played;
        private void OnFinishedMarkClick() => Episode.Status = EpisodeStatus.Unplayed;

        private void OnClick()
        {
            PodcastCollection.Current = Episode;
            Navigator.NavigateTo("/now-playing");
        }
	}
}
