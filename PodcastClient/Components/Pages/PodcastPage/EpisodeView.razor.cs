using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
	public partial class EpisodeView
	{
		[Parameter, EditorRequired]
		public Episode Episode { get; set; }
		public MarkupString Description { get; set; }

		protected override void OnInitialized()
		{
			Description = new(Episode.Description);
		}

		private void OnClick()
		{
			Navigator.NavigateTo($"/podcast/{Episode.PodcastId}/episode/{Episode.EpisodeNumber}");
		}
	}
}
