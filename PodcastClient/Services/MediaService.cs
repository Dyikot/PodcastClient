namespace PodcastClient.Services
{
	public class MediaService
	{
		public double AudioVolume { get; set; }
		public double VideoVolume { get; set; }
		public double AudioSpeed { get; set; }
		public double VideoSpeed { get; set; }

		public void Initialize(double? audioVolume, double? videoVolume,
							   double? audioSpeed, double? videoSpeed)
		{
			AudioVolume = audioVolume ?? 1;
			VideoVolume = videoVolume ?? 1;
			AudioSpeed = audioSpeed ?? 1;
			VideoSpeed = videoSpeed ?? 1;
		}
	}
}
