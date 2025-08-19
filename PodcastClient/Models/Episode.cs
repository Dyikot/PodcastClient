using Microsoft.AspNetCore.Components;

namespace PodcastClient.Models
{
	public enum EpisodeStatus
	{
		Unplayed, InProgress, Played
	}

	public enum EpisodeType
	{
		Audio, Video
	}

	public partial class Episode
	{
		private Uri? _iconUri;

		public required Guid Id { get; set; }
		public required string Name { get; set; }
		public required MarkupString Description { get; set; }
		public required DateTime ReleaseDate { get; set; }
		public required Uri Link { get; set; }
		public required TimeSpan Duration { get; set; }
		public required Uri ContentUri { get; set; }

		public int Number { get; set; }
		public Podcast? Podcast { get; set; }
		public TimeSpan Played { get; set; }
		public Uri? IconUri { get => _iconUri ?? Podcast?.IconUrl; set => _iconUri = value; }
		public EpisodeStatus Status { get; set; } = EpisodeStatus.Unplayed;
		public EpisodeType Type { get; set; } = EpisodeType.Audio;
	}
}
