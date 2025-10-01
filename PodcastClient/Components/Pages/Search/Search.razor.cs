using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Search
{
	public partial class Search : IAsyncDisposable
	{
		private const int ResultsPerPage = 50;

		private ElementReference _inputText = default!;
		private ApplicationDbContext _context = default!;
		private bool _searching = false;
		private string? _searchValue;
		private int _page = 1;

		public List<Podcast>? Results { get; private set; }
		public bool HasMoreResults { get; private set; } = false;

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
				_page = 1;
				_searching = false;
				Results = null;
				HasMoreResults = false;
				return;
			}

			_page = 1;
			Results = await PerformSearchAsync();			
		}

		private async Task<List<Podcast>> PerformSearchAsync(int skip = 0)
		{
			_searching = true;
			var results = await _context.Podcasts
				.Where(p => p.Title.StartsWith(_searchValue!))
				.Skip(skip)
				.Take(ResultsPerPage + 1)
				.ToListAsync();
			_searching = false;

			HasMoreResults = results.Count > ResultsPerPage;
			if (HasMoreResults)
			{
				results.RemoveAt(results.Count - 1);
			}

			return results;
		}

		private async Task LoadMoreAsync()
		{
			var results = await PerformSearchAsync(skip: _page++ * ResultsPerPage);
			Results!.AddRange(results);
		}
	}
}
