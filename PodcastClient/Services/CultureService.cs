using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace PodcastClient.Services
{
	public class CultureService
	{
		private readonly NavigationManager _navigationManager;

		public CultureService(NavigationManager navigationManager)
		{
			_navigationManager = navigationManager;
		}

		public CultureInfo Culture
		{
			get => CultureInfo.CurrentCulture;
			set
			{
				if (CultureInfo.CurrentCulture.Name != value.Name)
				{
					var uri = new Uri(_navigationManager.Uri).GetComponents(
						UriComponents.PathAndQuery,
						UriFormat.Unescaped
					);
					var cultureEscaped = Uri.EscapeDataString(value.Name);
					var uriEscaped = Uri.EscapeDataString(uri);

					_navigationManager.NavigateTo(
						uri: $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}",
						forceLoad: true
					);
				}
			}
		}
	}
}
