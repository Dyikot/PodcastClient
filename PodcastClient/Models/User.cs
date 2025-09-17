namespace PodcastClient.Models
{
	public class User
	{
		public int Id { get; set; }

		public List<Podcast> Podcasts { get; set; } = [];
		public List<UserEpisode> Episodes { get; set; } = [];
	}
}
