using Microsoft.AspNetCore.Components;
using PodcastClient.Models;
using System.Security.Cryptography;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public partial class EpisodeBlock
    {
        [Parameter, EditorRequired]
        public Episode Episode { get; set; }
        MarkupString Description { get; set; }

		protected override void OnInitialized()
		{
			base.OnInitialized();
			Description = new MarkupString($"<span class=\"about-body-description-text\">{Episode!.Description}</span>");
		}

		private void OnNotStartedMarkClick() => Episode.Status = PlayStatus.Played;
        private void OnFinishedMarkClick() => Episode.Status = PlayStatus.Unplayed;

        private void OnClick()
        {
            PodcastService.Playing = Episode;
            Navigator.NavigateTo("/now-playing");
        }
	}
}
