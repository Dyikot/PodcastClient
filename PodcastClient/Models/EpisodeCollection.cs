using System.Collections.ObjectModel;

namespace PodcastClient.Models
{
    public class EpisodeCollection : Collection<Episode>
    {
        private readonly Podcast _podcast;

        public EpisodeCollection(Podcast podcast)
        {
            _podcast = podcast;
		}

        public EpisodeCollection(Podcast source, IList<Episode> episodes):
            base(episodes)
        {
            _podcast = source;
            
            foreach(var episode in Items)
            {
                episode.Podcast = _podcast;
            }
        }

		protected override void ClearItems()
		{
			foreach (var episode in Items)
			{
				episode.Podcast = null;
			}

			base.ClearItems();
		}

		protected override void InsertItem(int index, Episode item)
		{
            item.Podcast = _podcast;
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, Episode item)
		{
            item.Podcast = _podcast;
			base.SetItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			var episode = Items[index];
			episode.Podcast = null;
			base.RemoveItem(index);
		}
    }
}
