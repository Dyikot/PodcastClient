using Microsoft.AspNetCore.Components;
using PodcastClient.Data;

namespace PodcastClient.Models
{
	public partial class Episode
	{
		private Uri? _iconUri;

		public required string Title { get; set; }
		public required MarkupString Description { get; set; }
		public required DateTime ReleaseDate { get; set; }
		public required Uri Link { get; set; }
		public required TimeSpan Duration { get; set; }
		public required Uri ContentUri { get; set; }

		public int Number { get; set; }
		public Podcast? Podcast { get; set; }
		public TimeSpan Played { get; set; }
		public EpisodeStatus Status { get; set; } = EpisodeStatus.Unplayed;
		public EpisodeType Type { get; set; } = EpisodeType.Audio;

		public Uri? IconUri 
		{
			get => _iconUri ??= Podcast!.IconUrl; 
			set => _iconUri = value; 
		}
	}

	public enum EpisodeStatus
	{
		Unplayed, InProgress, Played
	}

	public enum EpisodeType
	{
		Audio, Video
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
			{
				return 0;
			}			
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
