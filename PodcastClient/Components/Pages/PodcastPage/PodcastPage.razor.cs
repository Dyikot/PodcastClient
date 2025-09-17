using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.PodcastPage
{
	public partial class PodcastPage
    {
		public static readonly SortOrder[] SortOrderOptions = Enum.GetValues<SortOrder>();

        private EpisodeStatus? _episodesFilter;
        private SortOrder _sortOrder = SortOrder.Latest;

		[Parameter]
		public int PodcastId { get; set; }

		public Podcast? Podcast { get; private set; }

		public List<Episode>? Episodes { get; private set; }
		public List<Episode>? EpisodesSource { get; private set; }
        public List<UserEpisode>? UserEpisodes { get; private set; }
		public List<UserEpisode>? UserEpisodesSource { get; private set; }

        public bool UserHasSubscription => UserEpisodes != null;
        public bool IsInProgressFilter => _episodesFilter == EpisodeStatus.InProgress;
        public bool IsPlayedFilter => _episodesFilter == EpisodeStatus.Played;
        public bool IsUnplayedFilter => _episodesFilter == EpisodeStatus.Unplayed;

        public MarkupString? Description { get; set; }

        public SortOrder SortOrder
        {
            get => _sortOrder;
            set
            {
                if(_sortOrder != value)
                {
                    _sortOrder = value;
                    SortItems();
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var context = DbContextFactory.CreateDbContext();

            Podcast = await context.Podcasts
                .AsNoTracking()
                .Include(p => p.Episodes)
                .FirstOrDefaultAsync(p => p.Id == PodcastId);

            Episodes = EpisodesSource = Podcast?.Episodes;
			if (Podcast != null)
            {
                Description = new(Podcast.Description);
                UserEpisodes = UserEpisodesSource = await QueryUserEpisodesAsync(context);
			}

            SortItems();
        }

		private void OnInProgressButtonClick() => OnFilterButtonClick(EpisodeStatus.InProgress);
		private void OnPlayedButtonClick() => OnFilterButtonClick(EpisodeStatus.Played);
		private void OnUnplayedButtonClick() => OnFilterButtonClick(EpisodeStatus.Unplayed);

		private void OnFilterButtonClick(EpisodeStatus filter)
        {
			if (_episodesFilter == filter)
			{
				_episodesFilter = null;
			}
			else
			{
				_episodesFilter = filter;
			}

            FilterItems();
            SortItems();
		}

        private async Task OnSubscribeButtonClick()
        {
            if(UserHasSubscription)
            {
                await UserContext.RemovePodcastAsync(PodcastId);

                UserEpisodes = UserEpisodesSource = null;
                _episodesFilter = null;
                _sortOrder = SortOrder.Latest;
            }
            else
            {

            }
        }

        private void SortItems()
        {
            if(UserHasSubscription)
            {
                UserEpisodes?.Sort(new UserEpisodeSortOrder(SortOrder));
            }
            else
            {
                Episodes?.Sort(new EpisodeSortOrder(SortOrder));
            }
        }

        private void FilterItems()
        {
            if(UserHasSubscription)
            {
                if(_episodesFilter != null)
                {
                    UserEpisodes = UserEpisodesSource!
                        .Where(ep => ep.Status == _episodesFilter)
                        .ToList();
                }
                else
                {
                    UserEpisodes = UserEpisodesSource;
				}
            }
        }

        private async Task<List<UserEpisode>?> QueryUserEpisodesAsync(ApplicationDbContext context)
        {
			var episodes = await context.UserEpisodes
					.AsNoTracking()
					.Where(ep => ep.UserId == UserContext.UserId &&
								 ep.PodcastId == PodcastId)
					.ToListAsync();

			for (int i = 0; i < episodes.Count; i++)
			{
				episodes[i].Episode = Podcast!.Episodes[i];
			}

            return episodes.Count > 0 ? episodes : null;
		}

		private async Task OnEpisodeStatusChanged(UserEpisode userEpisode)
		{
            using var context = DbContextFactory.CreateDbContext();
            context.UserEpisodes.Update(userEpisode);
            await context.SaveChangesAsync();
		}
    }
}
