using Microsoft.AspNetCore.Components;

namespace PodcastClient.Models
{
	public enum PlayStatus
	{
		Unplayed, InProgress, Played
	}

	public enum ContentType
	{
		Audio, Video
	}

	public class Episode
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
		public Podcast? Channel { get; set; }
		public TimeSpan Played { get; set; }
		public Uri? IconUri { get => _iconUri ?? Channel?.IconUrl; set => _iconUri = value; }
		public PlayStatus Status { get; set; } = PlayStatus.Unplayed;
		public ContentType Type { get; set; } = ContentType.Audio;
	}
}
