using Microsoft.AspNetCore.Components;
using PodcastClient.Data;

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

		private readonly EqualityComparer<TItem> _comparer = EqualityComparer<TItem>.Default;
		private List<ItemPresenter> _itemPrenseters = default!;
		private TItem _selectedItem = default!;

		[Parameter]
		public string ButtonClass { get; set; } = string.Empty;
		[Parameter]
		public string ItemClass { get; set; } = string.Empty;
		[Parameter]
		public string DropDownClass { get; set; } = string.Empty;

		[Parameter]
		public EventCallback<TItem> SelectedItemChanged { get; set; }
		[Parameter]
		public EventCallback<TItem> SelectionChanged { get; set; }
		[Parameter]
		public EventCallback<TItem> OnItemClicked { get; set; }

		[Parameter]
		public RenderFragment<TItem>? ButtonContentTemplate { get; set; }
		[Parameter]
		public RenderFragment<TItem> ItemTemplate { get; set; } = DataTemplates.StringTemplate;

		[Parameter, EditorRequired]
		public TItem SelectedItem
		{
			get => _selectedItem;
			set
			{
				if(!_comparer.Equals(_selectedItem, value))
				{
					_selectedItem = value;

					if(SelectedItemChanged.HasDelegate)
					{
						SelectedItemChanged.InvokeAsync(value);
					}
				}
			}
		}

		[Parameter, EditorRequired]
		public IEnumerable<TItem> Items { get; set; }

		[Parameter]
		public DropDownPosition DropDownPosition { get; set; } = DropDownPosition.Bottom;

		public bool IsMouseOver { get; private set; } = false;
		public bool IsDropDownOpen { get; private set; } = false;

		private RenderFragment<TItem> DefaultButtonContentTemplate => item =>
		{
			return builder =>
			{
				builder.OpenElement(0, "div");
				builder.AddAttribute(1, "class", "d-flex align-items-center justify-content-between px-1");
				builder.AddContent(2, ItemTemplate(item));
				builder.OpenElement(3, "img");
				builder.AddAttribute(4, "src", "Controls/ComboBox/ArrowDown.svg");
				builder.CloseElement();
				builder.CloseElement();
			};
		};

		protected override void OnInitialized()
		{
			ButtonContentTemplate ??= DefaultButtonContentTemplate;
			_itemPrenseters = Items.Select(item => new ItemPresenter(item, this)).ToList();
		}

		private void OnButtonClick() => IsDropDownOpen = !IsDropDownOpen;
		private void OnMouseEnter() => IsMouseOver = true;
		private void OnMouseLeave() => IsMouseOver = false;

		private async Task OnItemClick(TItem item)
		{
			IsDropDownOpen = false;

			if(OnItemClicked.HasDelegate)
			{
				await OnItemClicked.InvokeAsync(item);
			}

			if (_comparer.Equals(SelectedItem, item))
			{
				return;
			}

			SelectedItem = item;

			if(SelectionChanged.HasDelegate)
			{
				await SelectionChanged.InvokeAsync(item);
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
