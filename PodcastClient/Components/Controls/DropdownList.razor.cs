using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
	public partial class DropdownList<TValue>
	{
		private bool _isVisible = false;
		private bool _isOverPanel = false;
		private Dictionary<TValue, ElementReference> _items;

		[Parameter, EditorRequired]
		public TValue Value { get; set; }
		[Parameter]
		public RenderFragment? ChildContent { get; set; }
		[Parameter]
		public string ButtonClass { get; set; } = string.Empty;
		[Parameter]
		public string ItemClass { get; set; } = string.Empty;
		[Parameter]
		public string PanelClass { get; set; } = string.Empty;
		[Parameter, EditorRequired]
		public Dictionary<TValue, string> ValueContentDictionary { get; set; }
		[Parameter]
		public EventCallback<TValue> ItemClick { get; set; }

		protected override void OnInitialized()
		{
			if(_items == null)
			{
				_items = new Dictionary<TValue, ElementReference>(ValueContentDictionary.Count);
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (_isVisible)
			{
				await _items[Value].FocusAsync();
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		private void OnButtonClick() => _isVisible = true;

		private async Task OnItemClick(TValue value)
		{
			Value = value;
			_isVisible = false;

			if(ItemClick.HasDelegate)
			{
				await ItemClick.InvokeAsync(value);
			}
		}

		private void OnItemFocusLost()
		{
			if (!_isOverPanel)
			{
				_isVisible = false;
			}
		}
	}
}
