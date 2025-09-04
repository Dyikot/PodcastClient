using PodcastClient.Models;

namespace PodcastClient.Components.Pages.NowPlaying
{
    public partial class NowPlaying
    {
        public Episode? Episode { get; set; }

        protected override void OnParametersSet()
        {
            Episode = PodcastCollection.Current;
        }

        private void OnPlay()
        {
            if(Episode != null && Episode.Status != EpisodeStatus.Played)
            {
                Episode.Status = EpisodeStatus.InProgress;
            }
        }

		private void OnPlayed()
		{
			if (Episode != null)
			{
				Episode.Status = EpisodeStatus.Played;
			}
		}
	}
}
