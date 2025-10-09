using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using PodcastClient.Components.Controls;
using PodcastClient.Data;
using PodcastClient.Services;

namespace PodcastClient.Components.Pages.Home
{
	public partial class Home
    {
		private readonly DateOnly _newDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3));
		private Carousel _popularPodcasts = default!;
		private Carousel _newPodcasts = default!;

		public List<Podcast> PopularPodcasts { get; set; } = default!;
		public List<Podcast> NewPodcasts { get; set; } = default!;
		public List<EpisodeViewModel>? NewEpisodes { get; set; }
		public List<CategoryViewModel> Categories { get; set; } = default!;

		protected override async Task OnInitializedAsync()
		{
			Categories = CategoriesService.Categories
				.Select(c => new CategoryViewModel(Localizer, c))
				.ToList();

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
					.Where(p => p.Inserted > _newDate)
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
	}

	public class CategoryViewModel
	{
		private readonly IStringLocalizer _localizer;
		private readonly string _resourceKey;

		public CategoryViewModel(IStringLocalizer localizer, CategoryInfo categoryInfo)
		{
			_localizer = localizer;
			_resourceKey = categoryInfo.ResourceKey;
			Uri = categoryInfo.Uri;
			ImageSource = categoryInfo.ImageSource;
		}

		public string LocalizedName => _localizer[_resourceKey];
		public string Uri { get; set; }
		public string ImageSource { get; set; }
	}
}
