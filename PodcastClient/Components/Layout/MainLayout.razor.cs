using PodcastClient.Extensions;

namespace PodcastClient.Components.Layout
{
	public partial class MainLayout
	{
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if(firstRender)
			{
				var theme = await LocalStorage.GetTheme();
				var culture = await LocalStorage.GetCulture();

				if(theme != null)
				{
					ThemeService.Theme = theme;
				}

				if(culture != null)
				{
					CultureService.Culture = culture;
				}
			}

			await base.OnAfterRenderAsync(firstRender);
		}
	}
}
