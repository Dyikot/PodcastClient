namespace PodcastClient.Services
{
    public class ThemeService
    {
        public const string Dark = "dark";
        public const string Light = "light";

        private string _active = Light;

		public string Theme
        {
            get => _active; 
            set
            {
                if(_active != value)
                {
                    _active = value;
                }
            }
        }
	}
}
