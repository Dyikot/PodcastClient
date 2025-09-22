using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class MyPodcasts
    {
		public List<Podcast> Podcasts { get; set; } = [];
		public string RssFeed { get; set; } = string.Empty;

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
				try
				{
					var rss = new Uri(RssFeed);
					var podcast = await PodcastsService.FindAsync(p => p.Rss == rss);

					if(podcast == null)
					{
						podcast = await PodcastRssFetcher.GetPodcastAsync(rss);
						if(podcast != null)
							await PodcastsService.AddPodcastAsync(podcast);
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

				RssFeed = string.Empty;
            }
		}
	}
}
