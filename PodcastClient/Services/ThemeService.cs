namespace PodcastClient.Services
{
    public enum Theme
    {
        Light, Dark
    }

    public class ThemeService
    {
        public Theme Active { get; set; } = Theme.Dark;
        public string CssClass => $"theme-{Active.ToString().ToLower()}";
	}
}
