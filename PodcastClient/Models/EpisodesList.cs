using System.Collections;

namespace PodcastClient.Models
{
    public class EpisodesList: IList<Episode>
    {
        private readonly Podcast _source;
        private readonly List<Episode> _episodes = [];

        public EpisodesList(Podcast source) => _source = source;
        public EpisodesList(IEnumerable<Episode> episodes, Podcast source)
        {
            _source = source;
            _episodes = new List<Episode>(episodes);
            _episodes.ForEach(episode => episode.Channel = _source);
        }

        public int Count => _episodes.Count;
        public bool IsReadOnly => false;        

        public void Add(Episode episode)
        {
            _episodes.Add(episode);
            episode.Channel = _source;
        }

        public void Insert(int index, Episode episode)
        {
            episode.Channel = _source;
            _episodes.Insert(index, episode);
        }

        public void Clear() => _episodes.Clear();
        public bool Contains(Episode episode) => _episodes.Contains(episode);
        public bool Remove(Episode episode) => _episodes.Remove(episode);
        public void CopyTo(Episode[] array, int arrayIndex) => _episodes.CopyTo(array, arrayIndex);
        public IEnumerator<Episode> GetEnumerator() => _episodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _episodes.GetEnumerator();
        public int IndexOf(Episode episode) => _episodes.IndexOf(episode);
        public void RemoveAt(int index) => _episodes.RemoveAt(index);

        public Episode this[int index] { get => _episodes[index]; set => _episodes[index] = value; }
    }
}
