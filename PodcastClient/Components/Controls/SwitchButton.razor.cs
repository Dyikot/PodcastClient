using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
    public partial class SwitchButton
    {
        private bool _isActive = false;

        [Parameter]
        public string Class { get; set; }
		[Parameter]
		public string ActiveClass { get; set; }
		[Parameter, EditorRequired]
        public RenderFragment ContentTemplate { get; set; }
        [Parameter ,EditorRequired]
        public RenderFragment ActiveContentTemplate { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }
		[Parameter]
		public EventCallback<bool> OnStateChanged { get; set; }
		[Parameter]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive == value)
                {
                    return;
                }

                if(OnStateChanged.HasDelegate)
                {
                    OnStateChanged.InvokeAsync(value);
                }

                _isActive = value;
            }
        }

        private void OnButtonClick()
        {
            IsActive = !IsActive;

            if (OnClick.HasDelegate)
            {
                OnClick.InvokeAsync();
            }
        }
    }
}
