using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PodcastClient.Components.Controls;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Home
{
	public partial class Home
    {
		private readonly DateOnly _lastMonth = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
		private Carousel _popularPodcasts = default!;
		private Carousel _newPodcasts = default!;

		public List<Podcast> PopularPodcasts { get; set; } = default!;
		public List<Podcast> NewPodcasts { get; set; } = default!;
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

				PopularPodcasts = await content.Podcasts
					.AsNoTracking()
					.OrderByDescending(p => p.Subscribers)
					.Take(20)
					.ToListAsync();

				NewPodcasts = await content.Podcasts
					.AsNoTracking()
					.Where(p => p.Inserted > _lastMonth)
					.OrderByDescending(p => p.Subscribers)
					.Take(20)
					.ToListAsync();
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if(firstRender)
			{
				await JS.InvokeVoidAsync("InitializeHomePage", 
										 _popularPodcasts.Element, 
										 _newPodcasts.Element);
			}
		}

		private RenderFragment CategoryView(string name, string href, string src)
		{
			return builder =>
			{
				builder.OpenElement(0, "a");
				builder.AddAttribute(1, "href", href);
				builder.AddAttribute(2, "class", "bg-1 hoverable rounded-2 categoryButton");
				builder.OpenElement(3, "img");
				builder.AddAttribute(4, "src", src);
				builder.CloseElement();
				builder.AddContent(5, name);
				builder.CloseElement();
			};
		}
	}
}
