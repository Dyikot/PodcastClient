using Microsoft.AspNetCore.Components;
using PodcastClient.Data;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
	public partial class PodcastPage
    {
        public static readonly SortOrder[] SortOrderOptions = Enum.GetValues<SortOrder>();

        private EpisodeStatus? _filterByStatus;
        private SortOrder _sortOrder = SortOrder.Latest;

		[Parameter]
		public string Name { get; set; } = "";
		public Podcast? Podcast { get; set; }
        public List<Episode> Episodes { get; set; } = [];

        public bool IsInProgressButtonActive => _filterByStatus == EpisodeStatus.InProgress;
        public bool HasPlayedButtonActive => _filterByStatus == EpisodeStatus.Played;
        public bool HasUnplayedButtonActive => _filterByStatus == EpisodeStatus.Unplayed;

        public SortOrder SortOrder
        {
            get => _sortOrder;
            set
            {
                if(_sortOrder != value)
                {
                    _sortOrder = value;
                    UpdateEpisodes(Podcast!.Episodes);
                }
            }
        }

        protected override void OnInitialized()
        {
            Podcast = PodcastCollection.First(podcast => podcast.Title == Name);
            
            if(Podcast != null)
            {
                UpdateEpisodes(Podcast.Episodes);
			}
        }

        private void UpdateEpisodes(EpisodeCollection episodes)
        {
            if(_filterByStatus != null)
            {
			    Episodes = episodes
					    .Where(ep => ep.Status == _filterByStatus)
					    .Order(new EpisodeSortOrder(SortOrder))
					    .ToList();
            }
            else
            {
				Episodes = episodes
						.Order(new EpisodeSortOrder(SortOrder))
						.ToList();
			}
		}

		private void OnInProgressButtonClick() => OnFilterButtonClick(EpisodeStatus.InProgress);
		private void OnPlayedButtonClick() => OnFilterButtonClick(EpisodeStatus.Played);
		private void OnUnplayedButtonClick() => OnFilterButtonClick(EpisodeStatus.Unplayed);

		private void OnFilterButtonClick(EpisodeStatus edisodeStatus)
        {
			if (_filterByStatus == edisodeStatus)
			{
				_filterByStatus = null;
			}
			else
			{
				_filterByStatus = edisodeStatus;
			}

            UpdateEpisodes(Podcast!.Episodes);
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
