using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PodcastClient.Components.Controls
{
	public partial class Carousel<T>
	{
		private ElementReference _element;

		[Parameter]
		public string Class { get; set; } = string.Empty;

		[Parameter, EditorRequired]
		public string LeftButtonId { get; set; }

		[Parameter, EditorRequired]
		public string RightButtonId { get; set; }

		[Parameter, EditorRequired]
		public IEnumerable<T> Items { get; set; }

		[Parameter, EditorRequired]
		public RenderFragment<T> ItemTemplate { get; set; }		

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if(firstRender)
			{
				await JS.InvokeVoidAsync("InitializeCarousel", _element, LeftButtonId, RightButtonId);
			}
		}
	}
}
