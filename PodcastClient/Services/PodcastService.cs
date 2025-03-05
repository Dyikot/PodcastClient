using PodcastClient.Models;

namespace PodcastClient.Services
{
    public interface IPodcastService
    {
        public List<Podcast> Podcasts { get; }
        public Episode? Playing { get; set; }
        public Podcast? GetChannelById(Guid id);
    }

    public class PodcastService: IPodcastService
    {
        private readonly List<Podcast> _podcasts;

        public PodcastService() => _podcasts = [];
        public PodcastService(IEnumerable<Podcast> podcasts) => _podcasts = new List<Podcast>(podcasts);

        public Podcast? GetChannelById(Guid id) => _podcasts.Find(podcast => podcast.Id == id);
        public List<Podcast> Podcasts => _podcasts;
		public Episode? Playing { get; set; }
	}
}
