namespace PodcastClient.Data
{
	public class User
	{
		public int Id { get; set; }

		public string Email { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string HashPassword { get; set; } = string.Empty;

		public List<Podcast> Podcasts { get; set; }
		public List<UserEpisode> Episodes { get; set; }
	}
}
