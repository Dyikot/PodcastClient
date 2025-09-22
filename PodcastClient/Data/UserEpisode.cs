using PodcastClient.Base;

namespace PodcastClient.Data
{
	public class UserEpisode
	{
		public int UserId { get; set; }
		public User User { get; set; } = null!;

		public int EpisodeNumber { get; set; }
		public int PodcastId { get; set; }
		public Episode Episode { get; set; } = null!;

		public TimeSpan Played { get; set; }
		public EpisodeStatus Status { get; set; } = EpisodeStatus.Unplayed;
	}

	public enum EpisodeStatus
	{
		Unplayed, InProgress, Played
	}

	public enum EpisodeType
	{
		Audio, Video
	}

	public class UserEpisodeSortOrder : IComparer<UserEpisode>
	{
		private readonly SortOrder _sortOrder;

		public UserEpisodeSortOrder(SortOrder sortOrder)
		{
			_sortOrder = sortOrder;
		}

		public int Compare(UserEpisode? x, UserEpisode? y)
		{
			if (x == null || y == null)
				return 0;
			else if (x == null)
			{
				return -1;
			}
			else if (y == null)
			{
				return 1;
			}

			return _sortOrder switch
			{
				SortOrder.Oldest => x.Episode.ReleaseDate.CompareTo(y.Episode.ReleaseDate),
				SortOrder.Latest => y.Episode.ReleaseDate.CompareTo(x.Episode.ReleaseDate),
				_ => throw new NotImplementedException()
			};
		}
	}
}
