using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public partial class EpisodeView
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }

        public bool HasPlayed
        {
            get => Episode.Status == EpisodeStatus.Played;
            set
            {
                var status = value ? EpisodeStatus.Played : EpisodeStatus.Unplayed;

                if(status != Episode.Status)
                {
                    Episode.Status = status;
                }
            }
        }

        private void OnClick()
        {
            PodcastCollection.Current = Episode;
            Navigator.NavigateTo("/now-playing");
        }
	}
}
