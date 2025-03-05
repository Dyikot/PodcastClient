using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NowPlaying
{
    public partial class NowPlaying
    {
        public Episode? Episode { get; set; }
        public string Title => Episode?.Name ?? Localizer["NoPlayingPodcast"];

        protected override void OnParametersSet()
        {
            Episode = PodcastService.Playing;
            base.OnParametersSet();
        }

        private void OnPlay()
        {
            if(Episode != null && Episode.Status != PlayStatus.Played)
            {
                Episode.Status = PlayStatus.InProgress;
            }
        }

		private void OnPlayed()
		{
			if (Episode != null)
			{
				Episode.Status = PlayStatus.Played;
			}
		}
	}
}
