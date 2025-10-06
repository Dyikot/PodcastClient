using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Components.Controls;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Search
{
	public partial class Search : IAsyncDisposable
	{
		private const int ItemsPerPage = 50;

		private string? _searchValue;
		private bool _hasMoreItems = false;
		private ApplicationDbContext _context = default!;
		private ElementReference _inputText = default!;
		private LoadableItemsSource<Podcast> _loadableItemsSource = default!;

		public ValueTask DisposeAsync() => _context.DisposeAsync();

		protected override async Task OnInitializedAsync()
		{
			_context = await DbContextFactory.CreateDbContextAsync();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await _inputText.FocusAsync();
			}
		}

		private async Task OnValueChanged(ChangeEventArgs e)
		{
			_searchValue = e.Value as string;

			if (string.IsNullOrEmpty(_searchValue))
			{
				_hasMoreItems = false;
				_loadableItemsSource.Reset();
				return;
			}

			await _loadableItemsSource.ReloadItemsAsync();
		}

		private bool HasMoreItems() => _hasMoreItems;

		private async Task<List<Podcast>> LoadItemsAsync(int page)
		{
			var items = await _context.Podcasts
				.Where(p => p.Title.StartsWith(_searchValue!))
				.Skip(page * ItemsPerPage)
				.Take(ItemsPerPage + 1)
				.ToListAsync();

			_hasMoreItems = items.Count > ItemsPerPage;
			if (_hasMoreItems)
			{
				items.RemoveAt(items.Count - 1);
			}

			return items;
		}
	}
}
