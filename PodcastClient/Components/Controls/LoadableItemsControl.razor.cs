using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PodcastClient.Components.Templates;
using PodcastClient.Resources;

namespace PodcastClient.Components.Controls
{
	public partial class LoadableItemsControl<T>
	{
		private List<T>? _items;		

		[Parameter]
		public string Class { get; set; } = "d-flex flex-column gap-3";
		[Parameter, EditorRequired]
		public string ItemsClass { get; set; }
		[Parameter]
		public string ButonClass { get; set; } = "btn bg-1 hoverable align-self-center";

		[Parameter, EditorRequired]
		public RenderFragment<T> ItemTemplate { get; set; }
		[Parameter]
		public RenderFragment NoItemsTemplate { get; set; } = DataTemplates.EmptyTemplate;
		[Parameter]
		public RenderFragment LoadingTemplate { get; set; } = DataTemplates.EmptyTemplate;
		[Parameter]
		public RenderFragment ButtonContentTemplate { get; set; } = ButtonContentDefaultTemplate;

		[Parameter]
		public bool LoadOnInitialization { get; set; } = false;
		[Parameter, EditorRequired]
		public Func<bool> HasMoreItems { get; set; }
		[Parameter, EditorRequired]
		public Func<int, Task<List<T>>> LoadItemsAsync { get; set; }

		public int Page { get; private set; } = 0;
		public bool IsLoading { get; private set; } = false;
		public IReadOnlyList<T>? Items => _items;

		public async Task LoadMoreItemsAsync()
		{
			IsLoading = true;
			var items = await LoadItemsAsync(Page++);
			IsLoading = false;

			_items ??= [];
			_items.AddRange(items);
		}

		public async Task ReloadItemsAsync()
		{
			Reset();
			await LoadMoreItemsAsync();
		}

		public void Reset()
		{
			_items = null;
			Page = 0;
		}

		protected override async Task OnInitializedAsync()
		{
			if (LoadOnInitialization)
			{
				await LoadMoreItemsAsync();
			}
		}

		private static void ButtonContentDefaultTemplate(RenderTreeBuilder builder)
		{
			builder.AddContent(0, Resource.ShowMore);
		}
	}
}
