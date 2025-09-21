using Microsoft.AspNetCore.Components;
using PodcastClient.Components.Controls;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public partial class UserEpisodeView
    {
        private Expander? _expander;

        [Parameter, EditorRequired]
        public UserEpisode UserEpisode { get; set; }
        public Episode Episode => UserEpisode.Episode;
		public MarkupString Description { get; set; }

		[Parameter]
        public EventCallback<UserEpisode> StatusChanged { get; set; }

        public bool HasPlayed => UserEpisode.Status == EpisodeStatus.Played;

		public async Task SetEpisodeStatusAsync(EpisodeStatus status)
        {
            if(UserEpisode.Status != status)
            {
                UserEpisode.Status = status;

				if (StatusChanged.HasDelegate)
				{
					await StatusChanged.InvokeAsync(UserEpisode);
				}
			}
        }

		protected override void OnInitialized()
		{
			Description = new(Episode.Description);
		}

        private async Task OnMoreClick()
        {
            _expander!.IsDropDownOpen = false;
            var status = HasPlayed ? EpisodeStatus.Unplayed : EpisodeStatus.Played;
            await SetEpisodeStatusAsync(status);
		}

		private void OnClick()
        {           
            Navigator.NavigateTo($"/podcast/{UserEpisode.PodcastId}/episode/{UserEpisode.EpisodeNumber}");
        }
	}
}
