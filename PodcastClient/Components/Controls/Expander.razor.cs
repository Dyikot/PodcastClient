using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
	public partial class Expander
	{
		[Parameter]
		public string Class { get; set; } = string.Empty;
		[Parameter]
		public string HeaderClass { get; set; } = string.Empty;
		[Parameter]
		public string ContentClass { get; set; } = string.Empty;

		[Parameter, EditorRequired]
		public RenderFragment HeaderTemplate { get; set; }
		[Parameter, EditorRequired]
		public RenderFragment ContentTemplate { get; set; }
		
		[Parameter]
		public DropDownPosition DropDownPosition { get; set; } = DropDownPosition.Bottom;

		[Parameter]
		public bool Autohide { get; set; } = true;

		public bool IsMouseOver { get; private set; } = false;
		public bool IsDropDownOpen { get; set; } = false;

		private void OnButtonClick() => IsDropDownOpen = !IsDropDownOpen;
		private void OnMouseEnter() => IsMouseOver = true;
		private void OnMouseLeave() => IsMouseOver = false;

		private void OnLostFocus()
		{
			if (Autohide && !IsMouseOver)
			{
				IsDropDownOpen = false;
			}
		}
	}
}
