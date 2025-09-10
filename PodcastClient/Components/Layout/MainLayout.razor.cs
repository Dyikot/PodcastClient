using PodcastClient.Data;
using PodcastClient.Services;

namespace PodcastClient.Components.Layout
{
	public partial class MainLayout
	{
		private bool _initialized = false;

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender && !_initialized)
			{
				var theme = await LocalStorage.GetItem(LocalStorageKeys.Theme);
				var audioVolume = await LocalStorage.GetItem<double?>(LocalStorageKeys.AudioVolume);
				var videoVolume = await LocalStorage.GetItem<double?>(LocalStorageKeys.VideoVolume);
				var audioSpeed = await LocalStorage.GetItem<double?>(LocalStorageKeys.AudioSpeed);
				var videoSpeed = await LocalStorage.GetItem<double?>(LocalStorageKeys.VideoSpeed);

				ThemeService.Initialize(theme);
				MediaService.Initialize(audioVolume, videoVolume, audioSpeed, videoSpeed);
				
				_initialized = true;
				StateHasChanged();
			}

			await base.OnAfterRenderAsync(firstRender);
		}
	}
}
