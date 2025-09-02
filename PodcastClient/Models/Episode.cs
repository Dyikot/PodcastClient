using Microsoft.AspNetCore.Components;

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
}
