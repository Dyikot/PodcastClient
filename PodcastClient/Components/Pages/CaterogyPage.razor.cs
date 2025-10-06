using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Components.Controls;
using PodcastClient.Data;
using PodcastClient.Resources;

namespace PodcastClient.Components.Pages
{
	public partial class CaterogyPage : IAsyncDisposable
	{
		private const int ItemsPerPage = 50;
		private bool _hasMoreItems = false;
		private ApplicationDbContext _context = default!;
		private LoadableItemsSource<int> _loadableItemsSource = default!;

		[Parameter]
		public string Category { get; set; } = default!;

		public CategoryInfo? CategoryInfo { get; set; }
		public string LocalizedName
		{
			get
			{
				if(CategoryInfo != null)
				{
					return Localizer[CategoryInfo.ResourceKey];
				}

				return Resource.CategoryNotFound;
			}
		}

		public async ValueTask DisposeAsync()
		{
			if (_context != null)
			{
				await _context.DisposeAsync();
			}
		}

		protected override void OnInitialized()
		{
			var categoryUri = $"category/{Category}";
			CategoryInfo = CategoriesService.Categories.Find(c => c.Uri == categoryUri);

			if(CategoryInfo != null)
			{
				_context = DbContextFactory.CreateDbContext();
			}
		}

		private bool HasMoreItems() => _hasMoreItems;

		private async Task<List<Podcast>> LoadItemsAsync(int page)
		{
			var podcasts = await _context.Podcasts
				.AsNoTracking()
				.Include(p =>  p.Categories)
				.Where(c => c.Categories.Any(c => c.Name == CategoryInfo!.Name))
				.Skip(page * ItemsPerPage)
				.Take(ItemsPerPage + 1)
				.ToListAsync();

			_hasMoreItems = podcasts.Count > ItemsPerPage;
			if (_hasMoreItems)
			{
				podcasts.RemoveAt(podcasts.Count - 1);
			}

			return podcasts;
		}
	}
}
