using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace PodcastClient.Components.Controls
{
	public partial class VideoPlayer : PlayerBase
	{
		private const string SpaceKey = " ";

		protected override async Task InitializeControlAsync()
		{
			await JS.InvokeVoidAsync("InitializeVideo", _control);
		}

		protected override async Task<double> GetCurrentTimeAsync()
		{
			return await JS.InvokeAsync<double>("GetVideoCurrentTime");
		}

		protected override async Task SetCurrentTimeAsync(TimeSpan value)
		{
			await JS.InvokeVoidAsync("SetVideoCurrentTime", value.TotalSeconds);
		}

		protected override async Task SetVolumeAsync(double value)
		{
			await JS.InvokeVoidAsync("SetVideoVolume", value);
		}

		protected override async Task SetPlaySpeedAsync(double value)
		{
			await JS.InvokeVoidAsync("SetVideoPlaySpeed", value);
		}

		protected override async Task PlayAsync()
		{
			await JS.InvokeVoidAsync("PlayVideo");
		}

		protected override async Task PauseAsync()
		{
			await JS.InvokeVoidAsync("PauseVideo");
		}

		private async Task OnFullscreenButtonClick()
		{
			await JS.InvokeVoidAsync("SwitchFullscreen");
		}

		private async Task OnKeyDown(KeyboardEventArgs e)
		{
			switch (e.Key)
			{
				case SpaceKey:
					await OnPlayButtonClick();
					break;

				case "ArrowLeft":
					await OnRewindButtonClick();
					break;

				case "ArrowRight":
					await OnForwardButtonClick();
					break;

				default:
					break;
			}
		}
	}
}
