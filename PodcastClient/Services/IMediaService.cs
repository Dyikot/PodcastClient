namespace PodcastClient.Services
{
	public interface IMediaService
	{
		public double AudioVolume { get; set; }
		public double VideoVolume { get; set; }
		public double AudioPlaySpeed { get; set; }
		public double VideoPlaySpeed { get; set; }
	}

	public class MediaService : IMediaService
	{
		public double AudioVolume { get; set; } = 1;
		public double VideoVolume { get; set; } = 1;
		public double AudioPlaySpeed { get; set; } = 1;
		public double VideoPlaySpeed { get; set; } = 1;
	}
}
