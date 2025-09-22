using Microsoft.AspNetCore.Components;
using PodcastClient.Components.Controls;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.PodcastPage
{
	public partial class EpisodeView
	{
		private Expander? _expander;

		[Parameter, EditorRequired]
		public Episode Episode { get; set; }

		[Parameter]
		public UserEpisode? UserEpisode { get; set; }

		[Parameter]
		public EventCallback<UserEpisode> OnStatusChanged { get; set; }

		public MarkupString Description { get; set; }

		public bool HasPlayed => UserEpisode?.Status == EpisodeStatus.Played;
		public string Disabled => UserEpisode == null ? "disabled" : "";
		public string Href => $"podcast/{Episode.PodcastId}/episode/{Episode.EpisodeNumber}";

		protected override void OnInitialized()
		{
			Description = new(Episode.Description);
		}

        private async Task OnMarkPlayedClick()
        {
            _expander!.IsDropDownOpen = false;
			var status = HasPlayed ? EpisodeStatus.Unplayed : EpisodeStatus.Played;

			if (UserEpisode!.Status != status)
			{
				UserEpisode!.Status = status;

				if (OnStatusChanged.HasDelegate)
				{
					await OnStatusChanged.InvokeAsync(UserEpisode);
				}
			}
		}
	}
}
