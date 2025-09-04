using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
	public enum DropDownPosition
	{
		Left, Top, Right, Bottom
	}

	public partial class ComboBox<TItem>
	{
		private class ItemPresenter(TItem item, ComboBox<TItem> parent)
		{
			private readonly ComboBox<TItem> _parent = parent;

			public TItem Item { get; init; } = item;
			public bool IsSelected => EqualityComparer<TItem>.Default.Equals(Item, _parent.SelectedItem);

			public async Task OnClick() => await _parent.OnItemClick(Item);
		}

		private static readonly RenderFragment<TItem> DefaultItemTemplate = item =>
		{
			return builder =>
			{
				builder.OpenElement(0, "span");
				builder.AddContent(1, item?.ToString());
				builder.CloseElement();
			};
		};

		private List<ItemPresenter> _itemPrenseters = default!;

		public bool IsMouseOver { get; private set; } = false;
		public bool IsDropDownOpen { get; private set; } = false;

		[Parameter]
		public DropDownPosition DropDownPosition { get; set; } = DropDownPosition.Bottom;

		[Parameter]
		public TItem? SelectedItem { get; set; } 
		[Parameter, EditorRequired]
		public IEnumerable<TItem> Items { get; set; }

		[Parameter]
		public RenderFragment? ButtonContentTemplate { get; set; }
		[Parameter]
		public RenderFragment<TItem> ItemTemplate { get; set; } = DefaultItemTemplate;

		[Parameter]
		public string ButtonClass { get; set; } = string.Empty;
		[Parameter]
		public string ItemClass { get; set; } = string.Empty;
		[Parameter]
		public string DropDownClass { get; set; } = "defaultBorder bg-1 rounded-2 py-1";

		[Parameter]
		public EventCallback<TItem> OnItemSelect { get; set; }

		protected override void OnInitialized()
		{
			SelectedItem ??= Items.First();
			_itemPrenseters = Items.Select(item => new ItemPresenter(item, this)).ToList();
		}

		private void OnButtonClick() => IsDropDownOpen = !IsDropDownOpen;
		private void OnMouseEnter() => IsMouseOver = true;
		private void OnMouseLeave() => IsMouseOver = false;

		private async Task OnItemClick(TItem item)
		{
			SelectedItem = item;
			IsDropDownOpen = false;

			if(OnItemSelect.HasDelegate)
			{
				await OnItemSelect.InvokeAsync(item);
			}
		}

		private void OnLostFocus()
		{
			if (!IsMouseOver)
			{
				IsDropDownOpen = false;
			}
		}
	}
}
