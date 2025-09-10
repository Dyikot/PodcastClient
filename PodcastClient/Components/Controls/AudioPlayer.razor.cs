using Microsoft.JSInterop;

namespace PodcastClient.Components.Controls
{
    public partial class AudioPlayer : PlayerBase
    {
		protected override async Task InitializeControlAsync()
		{
			await JS.InvokeVoidAsync("InitializeAudio", _control);
		}

		protected override async Task<double> GetCurrentTimeAsync()
        {
            return await JS.InvokeAsync<double>("GetCurrentTime");
		}

		protected override async Task SetCurrentTimeAsync(TimeSpan value)
        {
			await JS.InvokeVoidAsync("SetCurrentTime", value.TotalSeconds);
		}

		protected override async Task SetVolumeAsync(double value)
        {
			await JS.InvokeVoidAsync("SetVolume", value);
		}

		protected override async Task SetPlaySpeedAsync(double value)
        {
			await JS.InvokeVoidAsync("SetPlaySpeed", value);
		}

		protected override async Task PlayAsync()
        {
			await JS.InvokeVoidAsync("Play");
		}

		protected override async Task PauseAsync()
        {
			await JS.InvokeVoidAsync("Pause");
		}  		
	}
}
