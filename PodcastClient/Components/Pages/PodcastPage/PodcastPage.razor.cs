using Microsoft.AspNetCore.Components;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public enum SortOrder
    {
        Oldest, Latest
    }

    public partial class PodcastPage
    {
		[Parameter]
		public string Name { get; set; } = "";
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
                    return Podcast!.Episodes.Where(ep => ep.Status == EpisodeStatus.Played).ToList();
                }

                if (IsNotStartedButtonActive)
                {
                    return Podcast!.Episodes.Where(ep => ep.Status == EpisodeStatus.Unplayed).ToList();
                }

                if (IsInProgressButtonActive)
                {
					return Podcast!.Episodes.Where(ep => ep.Status == EpisodeStatus.InProgress).ToList();
				}

                return Podcast!.Episodes;
            }
        }

        protected override void OnInitialized()
        {
            Podcast = PodcastCollection.First(podcast => podcast.Title == Name);
            base.OnInitialized();
        }

        private void MarkAllItemsAs(EpisodeStatus progressStatus)
        {
            foreach (var episode in Podcast!.Episodes)
            {
                episode.Status = progressStatus;
            }
        }

        private void OnMarkAllItemsButtonClick() => MarkAllItemsAs(EpisodeStatus.Played);
        private void OnUnmarkAllItemsButtonClick() => MarkAllItemsAs(EpisodeStatus.Unplayed);

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
            if(PodcastCollection.Current != null && 
			   Podcast!.Episodes.Contains(PodcastCollection.Current))
            {
                PodcastCollection.Current = null; 
            }

            PodcastCollection.Remove(Podcast!);
			Navigator.NavigateTo("/");
		}
    }
}
