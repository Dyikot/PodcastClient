using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using PodcastClient.Components.Templates;

namespace PodcastClient.Components.Controls
{
	public partial class Carousel
	{
		private ElementReference _container;

		[Parameter]
		public string Class { get; set; } = string.Empty;
		[Parameter]
		public string ButtonClass { get; set; } = string.Empty;
		[Parameter]
		public string ContentClass { get; set; } = string.Empty;

		[Parameter]
		public RenderFragment LeftButtonTemplate { get; set; } = DefaultLeftButtonTemplate;
		[Parameter]
		public RenderFragment RightButtonTemplate { get; set; } = DefaultRightButtonTemplate;
		[Parameter]
		public RenderFragment ContentTemplate { get; set; } = DataTemplates.EmptyTemplate;

		public ElementReference Element { get; private set; }

		private static void DefaultLeftButtonTemplate(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "svg");
			builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
			builder.AddAttribute(2, "width", "32");
			builder.AddAttribute(3, "height", "32");
			builder.AddAttribute(4, "fill", "currentColor");
			builder.AddAttribute(5, "class", "bi bi-chevron-compact-left");
			builder.AddAttribute(6, "viewBox", "0 0 16 16");

			builder.OpenElement(7, "path");
			builder.AddAttribute(8, "fill-rule", "evenodd");
			builder.AddAttribute(9, "d", "M9.224 1.553a.5.5 0 0 1 .223.67L6.56 8l2.888 5.776a.5.5 0 1 1-.894.448l-3-6a.5.5 0 0 1 0-.448l3-6a.5.5 0 0 1 .67-.223");
			builder.CloseElement();

			builder.CloseElement();
		}

		private static void DefaultRightButtonTemplate(RenderTreeBuilder builder)
		{
			builder.OpenElement(0, "svg");
			builder.AddAttribute(1, "xmlns", "http://www.w3.org/2000/svg");
			builder.AddAttribute(2, "width", "32");
			builder.AddAttribute(3, "height", "32");
			builder.AddAttribute(4, "fill", "currentColor");
			builder.AddAttribute(5, "class", "bi bi-chevron-compact-right");
			builder.AddAttribute(6, "viewBox", "0 0 16 16");

			builder.OpenElement(7, "path");
			builder.AddAttribute(8, "fill-rule", "evenodd");
			builder.AddAttribute(9, "d", "M6.776 1.553a.5.5 0 0 1 .671.223l3 6a.5.5 0 0 1 0 .448l-3 6a.5.5 0 1 1-.894-.448L9.44 8 6.553 2.224a.5.5 0 0 1 .223-.671");
			builder.CloseElement();

			builder.CloseElement();
		}

		private async Task OnLeftButtonClick() => 
			await JS.InvokeVoidAsync("ScrollCarouselLeft", _container);

		private async Task OnRightButtonClick() => 
			await JS.InvokeVoidAsync("ScrollCarouselRight", _container);
	}
}
