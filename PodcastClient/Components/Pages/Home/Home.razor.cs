using Microsoft.EntityFrameworkCore;
using PodcastClient.Components.Pages.NewEpisodes;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Home
{
	public partial class Home
    {
		public List<EpisodeViewModel>? NewEpisodes { get; set; }

		protected override async Task OnInitializedAsync()
		{
			if (UserContext.IsAuthenticated)
			{
				using var content = DbContextFactory.CreateDbContext();
				NewEpisodes = await content.UserEpisodes
					.AsNoTracking()
					.Where(ue => ue.UserId == UserContext.UserId &&
								 ue.Status != EpisodeStatus.Played)
					.OrderByDescending(ue => ue.Episode.ReleaseDate)
					.Select(ue => new EpisodeViewModel
					{
						Episode = ue.Episode,
						PodcastTitle = ue.Episode.Podcast.Title
					})
					.Take(4)
					.ToListAsync();
			}
		}
    }
}
