namespace PodcastClient.Components.Layout
{
	public partial class NavMenu
	{
		private void SignOut() => NavigationManager.NavigateTo("Auth/SignOut", forceLoad: true);
		private void SignIn() => NavigationManager.NavigateTo("/signin");
	}
}
