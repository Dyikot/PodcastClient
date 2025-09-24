using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
    public partial class ToggleButton
    {
        private bool _isChecked = false;

        [Parameter]
        public string Class { get; set; } = string.Empty;
        [Parameter]
        public string CheckedClass { get; set; } = string.Empty;

		[Parameter, EditorRequired]
        public RenderFragment ContentTemplate { get; set; }
        [Parameter ,EditorRequired]
        public RenderFragment CheckedContentTemplate { get; set; }

        [Parameter]
        public EventCallback OnClick { get; set; }
		[Parameter]
		public EventCallback<bool> IsCheckedChanged { get; set; }

		[Parameter]
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
					if (IsCheckedChanged.HasDelegate)
					{
						IsCheckedChanged.InvokeAsync(value);
					}

                    _isChecked = value;                    
                }
            }
        }

        [Parameter]
        public bool IsEnabled { get; set; } = true;

        private string Classes => string.Join(" ",
            IsChecked ? CheckedClass : Class,
            IsEnabled ? "" : "disabled");


		private void OnButtonClick()
        {
            IsChecked = !IsChecked;

            if (OnClick.HasDelegate)
            {
                OnClick.InvokeAsync();
            }
        }
    }
}
