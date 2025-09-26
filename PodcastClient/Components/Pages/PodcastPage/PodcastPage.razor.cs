using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Base;
using PodcastClient.Data;
using System.Collections;

namespace PodcastClient.Components.Pages.PodcastPage
{
	public partial class PodcastPage
    {
		private const int ItemsPerPage = 25;
		public static readonly SortOrder[] SortOrderOptions = Enum.GetValues<SortOrder>();

		[SupplyParameterFromQuery]
		public int Page { get; set; }

		private EpisodeStatus? _episodesFilter;
        private SortOrder _sortOrder = SortOrder.Latest;

		[Parameter]
		public int PodcastId { get; set; }

		public Podcast? Podcast { get; private set; }

		public List<Episode>? Episodes { get; private set; }
		public List<Episode>? EpisodesSource { get; private set; }
        public List<UserEpisode>? UserEpisodes { get; private set; }
		public List<UserEpisode>? UserEpisodesSource { get; private set; }

		public int PageAmount
        {
            get
            {
                IList? episodes = UserHasSubscription ? UserEpisodes : Episodes;
				return (episodes!.Count + ItemsPerPage - 1) / ItemsPerPage;
			}
        }

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
            
            if(Podcast != null)
            {
                await SetContent(Podcast, context);
                SortItems();
            }

			if (Page < 0 || Page > PageAmount)
			{
				throw new NotSupportedException();
			}
			else if (Page == 0)
			{
				Page = 1;
			}
		}

        private async Task SetContent(Podcast podcast, ApplicationDbContext context)
        {
			Episodes = EpisodesSource = podcast.Episodes;
			Description = new(podcast.Description);
			UserEpisodes = UserEpisodesSource = await QueryUserEpisodesAsync(context);
		}

        private void ResetContent()
        {
			UserEpisodes = UserEpisodesSource = null;
			_episodesFilter = null;
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
                ResetContent();
                SortItems();
            }
            else
            {
                Podcast = await PodcastsService.FindAsync(PodcastId);
                await UserContext.AddPodcastAsync(Podcast!);

                using var context = DbContextFactory.CreateDbContext();
                await SetContent(Podcast!, context);
                SortItems();
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

            context.UserEpisodes.Attach(userEpisode);
            context.Entry(userEpisode).Property(ue => ue.Status).IsModified = true;
            await context.SaveChangesAsync();
		}
    }
}
