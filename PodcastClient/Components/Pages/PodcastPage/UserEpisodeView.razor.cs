using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public partial class UserEpisodeView
    {
        [Parameter, EditorRequired]
        public UserEpisode UserEpisode { get; set; }
        public Episode Episode => UserEpisode.Episode;
		public MarkupString Description { get; set; }

		[Parameter]
        public EventCallback<UserEpisode> StatusChanged { get; set; }

        public bool HasPlayed
        {
            get => UserEpisode.Status == EpisodeStatus.Played;
            set
            {
                var status = value ? EpisodeStatus.Played : EpisodeStatus.Unplayed;

                if(status != UserEpisode.Status)
                {
					UserEpisode.Status = status;

                    if(StatusChanged.HasDelegate)
                    {
                        StatusChanged.InvokeAsync(UserEpisode);
                    }
                }
            }
        }

		protected override void OnInitialized()
		{
			Description = new(Episode.Description);
		}

		private void OnClick()
        {           
            Navigator.NavigateTo($"/podcast/{UserEpisode.PodcastId}/episode/{UserEpisode.EpisodeNumber}");
        }
	}
}
