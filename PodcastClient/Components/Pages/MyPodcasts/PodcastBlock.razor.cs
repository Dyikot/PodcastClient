using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class PodcastBlock
    {
        [Parameter, EditorRequired]
        public Podcast Podcast { get; set; }
        [Parameter]
        public EventCallback<Podcast> Click { get; set; }
        public uint NewEpisodesNumber
        { 
            get
            {
                var newEpisodes = (uint)Podcast.NewEpisodes.Count;
                return newEpisodes > 99 ? 99 : newEpisodes;
            }
        }

        private Task OnClick() => Click.InvokeAsync(Podcast);
    }
}
