using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
    public partial class ToggleButton
    {
        private bool _isChecked = false;

        [Parameter]
        public string Class { get; set; }
		[Parameter]
		public string CheckedClass { get; set; }
		[Parameter, EditorRequired]
        public RenderFragment ContentTemplate { get; set; }
        [Parameter ,EditorRequired]
        public RenderFragment CheckedContentTemplate { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }
		[Parameter]
		public EventCallback<bool> OnStateChanged { get; set; }
		[Parameter]
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    if(OnStateChanged.HasDelegate)
                    {
                        OnStateChanged.InvokeAsync(value);
                    }

                    _isChecked = value;                    
                }
            }
        }

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
