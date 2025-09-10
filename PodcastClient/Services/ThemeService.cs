using Microsoft.JSInterop;

namespace PodcastClient.Services
{
    public class ThemeService
    {
        public const string Dark = "dark";
        public const string Light = "light";

        private string _theme = Light;
        private readonly IJSRuntime _js;

		public ThemeService(IJSRuntime js)
		{
			_js = js;
		}

		public string Theme
        {
            get => _theme;
            set
            {
                if(_theme != value)
                {
                    _theme = value;
                    _js.InvokeVoidAsync("SetTheme", value);
                }
            }
        }

        public void Initialize(string? theme) => _theme = theme ?? Light;
	}
}
