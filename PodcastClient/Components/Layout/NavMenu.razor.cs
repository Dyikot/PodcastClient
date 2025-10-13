using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Layout
{
	public partial class NavMenu
	{
		private bool _isAuthMenuOpen = false;
		private bool _isAuthMenuHover = false;

		private void OnSettingsClick()
		{
			_isAuthMenuOpen = false;
			NavigationManager.NavigateTo("settings");
		}

		private void LogIn()
		{
			_isAuthMenuOpen = false;
			NavigationManager.NavigateTo("signin");
		}

		private void LogOut() => NavigationManager.NavigateTo("auth/signout", forceLoad: true);

		private void OnAuthButtonClick()
		{
			_isAuthMenuOpen = !_isAuthMenuOpen;		
		}

		private void OnAuthMenuFocusLost()
		{
			if(!_isAuthMenuHover)
			{
				_isAuthMenuOpen = false;
			}
		}

		private void OnAuthMenuMouseLeave() => _isAuthMenuHover = false;
		private void OnAuthMenuMouseEnter() => _isAuthMenuHover = true;
	}
}
