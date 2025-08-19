using System.Collections.ObjectModel;

namespace PodcastClient.Models
{
	public class PodcastCollection : Collection<Podcast>
	{
		public Episode? Current { get; set; }
	}
}
