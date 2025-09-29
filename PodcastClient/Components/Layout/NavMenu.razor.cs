namespace PodcastClient.Components.Layout
{
	public partial class NavMenu
	{
		public void SignIn() => NavigationManager.NavigateTo("/signin");
		public void SignOut() => NavigationManager.NavigateTo("Auth/SignOut", forceLoad: true);
	}
}
