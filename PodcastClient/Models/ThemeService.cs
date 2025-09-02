namespace PodcastClient.Models
{
    public class ThemeService
    {
        public const string Dark = "dark";
        public const string Light = "light";

		public string Active { get; set; } = Light;
	}
}
