namespace PodcastClient.Data
{
	public class Podcast
	{
		public static Uri DefaultIconSource { get; } = new Uri("Rss.svg", UriKind.Relative);

		public int Id { get; set; }

		public required string Title { get; set; }
		public required string Author { get; set; }
		public required string Description { get; set; }
		public required Uri Rss { get; set; }
		public Uri IconSource { get; set; } = DefaultIconSource;
		public List<Episode> Episodes { get; set; } = [];
		public List<User> Users { get; set; } = [];
	}
}
