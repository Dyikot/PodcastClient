using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace PodcastClient.Components.Controls
{
    public partial class SwitchButton
    {
        private bool _isActive = false;

        [Parameter]
        public string ActiveClass { get; set; }
        [Parameter]
        public string NormalClass { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public EventCallback OnActivated { get; set; }
        [Parameter]
        public EventCallback OnDeactivated { get; set; }
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

                if(IsActiveChanged.HasDelegate)
                {
                    IsActiveChanged.InvokeAsync(value);
                }

                _isActive = value;
            }
        }
        [Parameter]
        public EventCallback<bool> IsActiveChanged { get; set; }

        private void OnButtonClick()
        {
            IsActive = !IsActive;

            if (IsActive && OnActivated.HasDelegate)
            {
                OnActivated.InvokeAsync();
            }
            else if(!IsActive && OnDeactivated.HasDelegate)
            {
                OnDeactivated.InvokeAsync();
            }
        }
    }
}
