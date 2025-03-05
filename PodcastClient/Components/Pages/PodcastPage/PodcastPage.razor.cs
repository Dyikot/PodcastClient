using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PodcastClient.Models;
using System.Globalization;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public enum SortOrder
    {
        Oldest, Latest
    }

    public partial class PodcastPage
    {
		[SupplyParameterFromQuery(Name ="id")]
        public Guid PodcastId { get; set; } 
        public Podcast? Podcast { get; set; }
        private SortOrder SortOrder { get; set; } = SortOrder.Latest;
        private bool IsInProgressButtonActive { get; set; } = false;
        private bool IsFinishedButtonActive { get; set; } = false;
        private bool IsNotStartedButtonActive { get; set; } = false;

        private IList<Episode> Episodes
        {
            get
            {
                if (IsFinishedButtonActive)
                {
                    return Podcast!.PlayedEpisodes;
                }

                if (IsNotStartedButtonActive)
                {
                    return Podcast!.UnplayedEpisodes;
                }

                if (IsInProgressButtonActive)
                {
                    return Podcast!.InProgressEpisodes;
                }

                return Podcast!.Episodes;
            }
        }

        protected override void OnInitialized()
        {
            Podcast = PodcastService.GetChannelById(PodcastId);
            base.OnInitialized();
        }

        private void MarkAllItemsAs(PlayStatus progressStatus)
        {
            foreach (var episode in Podcast!.Episodes)
            {
                episode.Status = progressStatus;
            }
        }

        private void OnMarkAllItemsButtonClick() => MarkAllItemsAs(PlayStatus.Played);
        private void OnUnmarkAllItemsButtonClick() => MarkAllItemsAs(PlayStatus.Unplayed);

        private void OnInProgressButtonClick()
        {
            IsFinishedButtonActive = false;
            IsNotStartedButtonActive = false;
        }

        private void OnFinishedButtonClick()
        {
            IsInProgressButtonActive = false;
            IsNotStartedButtonActive = false;
        }

        private void OnNotStartedButtonClick()
        {
            IsInProgressButtonActive = false;
            IsFinishedButtonActive = false;
        }        

        private void OnUnsubscribeButtonClick()
        {
            if(PodcastService.Playing != null &&
			   Podcast!.Episodes.Contains(PodcastService.Playing))
            {
                PodcastService.Playing = null; 
            }

            PodcastService.Podcasts.Remove(Podcast);
			Navigator.NavigateTo("/");
		}
    }
}
