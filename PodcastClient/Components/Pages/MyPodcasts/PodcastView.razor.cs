using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class PodcastView
    {
        [Parameter, EditorRequired]
        public Podcast Podcast { get; set; }
        [Parameter]
        public EventCallback<Podcast> Click { get; set; }

        public IList<Episode> NewEpisodes { get; private set; }

		protected override void OnInitialized()
		{
            NewEpisodes = Podcast.Episodes.Where(ep => ep.Status != EpisodeStatus.Played).ToList();
			base.OnInitialized();
		}

        private Task OnClick() => Click.InvokeAsync(Podcast);
    }
}
