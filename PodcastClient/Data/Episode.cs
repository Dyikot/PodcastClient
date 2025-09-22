using PodcastClient.Base;

namespace PodcastClient.Data
{
	public class Episode
	{
		public int EpisodeNumber { get; set; }

		public int PodcastId { get; set; }
		public Podcast Podcast { get; set; } = null!;

		public required string Title { get; set; }
		public required string Description { get; set; }
		public required DateTime ReleaseDate { get; set; }
		public required TimeSpan Duration { get; set; }
		public required Uri Source { get; set; }
		public required Uri IconSource { get; set; }
		public required Uri ContentSource { get; set; }
		public EpisodeType Type { get; set; } = EpisodeType.Audio;
	}

	public class EpisodeSortOrder : IComparer<Episode>
	{
		private readonly SortOrder _sortOrder;

		public EpisodeSortOrder(SortOrder sortOrder)
		{
			_sortOrder = sortOrder;
		}

		public int Compare(Episode? x, Episode? y)
		{
			if(x == null || y == null)
				return 0;
			else if(x == null)
			{
				return -1;
			}
			else if(y == null)
			{
				return 1;
			}

			return _sortOrder switch
			{
				SortOrder.Oldest => x.ReleaseDate.CompareTo(y.ReleaseDate),
				SortOrder.Latest => y.ReleaseDate.CompareTo(x.ReleaseDate),
				_ => throw new NotImplementedException()
			};
		}
	}
}
