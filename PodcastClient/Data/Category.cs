namespace PodcastClient.Data
{
	public class Category
	{
		public required string Name { get; set; }

		public List<Podcast> Podcasts { get; set; }
	}
}
