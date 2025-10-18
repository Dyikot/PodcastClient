namespace PodcastClient.Data
{
	public class Category
	{
		public Category(string name)
		{
			Name = name;
			Podcasts = null!;
		}

		public Category(string name, List<Podcast> podcasts)
		{
			Name = name;
			Podcasts = podcasts;
		}

		public string Name { get; set; }
		public List<Podcast> Podcasts { get; set; }
	}	
}
