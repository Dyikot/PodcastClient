using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Library
{
    public partial class Library
    {
		public List<Podcast> Podcasts { get; set; } = [];
		public string RssFeed { get; set; } = string.Empty;
		public bool IsAddingPodcast { get; private set; } = false;

		protected override async Task OnInitializedAsync()
		{
			using var context = DbContextFactory.CreateDbContext();

			var user = await context.Users
				.AsNoTracking()
				.Include(u => u.Podcasts)
				.SingleAsync(u => u.Id == UserContext.UserId);

			Podcasts = user.Podcasts;
		}

		private async Task OnSubmitButtonClick()
		{
			if (!string.IsNullOrEmpty(RssFeed))
			{
				IsAddingPodcast = true;
				await TryAddPodcast();
				IsAddingPodcast = false;
				RssFeed = string.Empty;
            }
		}

		private async Task TryAddPodcast()
		{
			try
			{				
				var rss = new Uri(RssFeed);

				if (Podcasts.Any(p => p.Rss == rss))
				{
					return;
				}

				var podcast = await PodcastsService.FindAsync(p => p.Rss == rss);

				if (podcast == null)
				{
					podcast = await PodcastRssFetcher.GetPodcastAsync(rss);
					if (podcast != null)
					{
						await PodcastsService.AddPodcastAsync(podcast);
					}
				}

				if (podcast != null)
				{
					await UserContext.AddPodcastAsync(podcast);
					Podcasts.Add(podcast);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError(ex.Message);
			}
		}
	}
}
