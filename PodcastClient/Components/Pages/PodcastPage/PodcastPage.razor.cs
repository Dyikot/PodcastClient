using Microsoft.AspNetCore.Components;
using PodcastClient.Components.Controls;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
    public enum SortOrder
    {
        Oldest, Latest
    }

    public partial class PodcastPage
    {
        public static readonly SortOrder[] SortOrderOptions = Enum.GetValues<SortOrder>();

        private EpisodeStatus? _filterByStatus;

		[Parameter]
		public string Name { get; set; } = "";
		public Podcast? Podcast { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.Latest;
        public bool IsInProgressButtonActive => _filterByStatus == EpisodeStatus.InProgress;
        public bool HasPlayedButtonActive => _filterByStatus == EpisodeStatus.Played;
        public bool HasUnplayedButtonActive => _filterByStatus == EpisodeStatus.Unplayed;

        public IList<Episode> Episodes
        {
            get
            {
                if(_filterByStatus == null)
                {
                    return Podcast!.Episodes;
				}

				return Podcast!.Episodes.Where(ep => ep.Status == _filterByStatus).ToList();
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
            if(_filterByStatus == EpisodeStatus.InProgress)
            {
                _filterByStatus = null;
            }
            else
            {
                _filterByStatus = EpisodeStatus.InProgress;
            }
        }

        private void OnPlayedButtonClick()
        {
			if (_filterByStatus == EpisodeStatus.Played)
			{
				_filterByStatus = null;
			}
			else
			{
				_filterByStatus = EpisodeStatus.Played;
			}
		}

        private void OnUnplayedButtonClick()
        {
			if (_filterByStatus == EpisodeStatus.Unplayed)
			{
				_filterByStatus = null;
			}
			else
			{
				_filterByStatus = EpisodeStatus.Unplayed;
			}
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
