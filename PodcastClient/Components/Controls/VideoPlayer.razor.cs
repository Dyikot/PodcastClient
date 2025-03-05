using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PodcastClient.Components.Controls
{
	public partial class VideoPlayer
	{
		public readonly Uri PlayIconUri = new Uri("Controls/VideoPlayer/Play.png", UriKind.Relative);
		public readonly Uri PauseIconUri = new Uri("Controls/VideoPlayer/Pause.png", UriKind.Relative);
		public readonly Uri VolumeIconUri = new Uri("Controls/VideoPlayer/Volume.png", UriKind.Relative);
		public readonly Uri NoVolumeIconUri = new Uri("Controls/VideoPlayer/NoVolume.png", UriKind.Relative);
		private ElementReference _videoPlayer;
		private TimeSpan _currentTime;
		private double _beforeMuteVolume = 0;
		private double _volume;
		private double _playSpeed;
		private bool _isMute = false;
		private bool _isPlaying = false;
		private bool _canPlay = false;
		private bool _initialized = false;
		private readonly Dictionary<double, string> SpeedContentDictionary = new()
		{
			{ 0.5, "0.5x" },
			{ 1.0, "1x" },
			{ 1.25, "1.25x"},
			{ 1.5, "1.5x"},
			{ 2.0, "2x" }
		};

		[Parameter]
		public Uri? Source { get; set; }

		[Parameter]
		public TimeSpan Duration { get; set; }

		[Parameter]
		public TimeSpan CurrentTime
		{
			get => _currentTime;
			set
			{
				if (CurrentTimeChanged.HasDelegate)
				{
					CurrentTimeChanged.InvokeAsync(value);
				}

				_currentTime = value;
			}
		}

		[Parameter, EditorRequired]
		public double Volume
		{
			get => _volume;
			set
			{
				if (VolumeChanged.HasDelegate)
				{
					VolumeChanged.InvokeAsync(value);
				}

				_volume = value;
			}
		}

		[Parameter, EditorRequired]
		public double PlaySpeed
		{
			get => _playSpeed;
			set
			{
				if (PlaySpeedChanged.HasDelegate)
				{
					PlaySpeedChanged.InvokeAsync(value);
				}

				_playSpeed = value;
			}
		}

		[Parameter]
		public EventCallback OnPlay { get; set; }
		[Parameter]
		public EventCallback OnPlayed { get; set; }
		[Parameter]
		public EventCallback<TimeSpan> CurrentTimeChanged { get; set; }
		[Parameter]
		public EventCallback<double> VolumeChanged { get; set; }
		[Parameter]
		public EventCallback<double> PlaySpeedChanged { get; set; }

		private string TimeFormat => Duration.Hours > 0 ? "hh':'mm':'ss" : "mm':'ss";
		private double VolumePersentage => (Volume / 1) * 100;
		private double DurationPersentage => (CurrentTime / Duration) * 100;

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			if (!_initialized)
			{
				_initialized = true;
				await base.SetParametersAsync(parameters);
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if(firstRender)
			{
				await JS.InvokeVoidAsync("InitializeVideo");
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task OnMetadataLoaded()
		{
			_canPlay = true;
			await _videoPlayer.FocusAsync();
			await JS.InvokeVoidAsync("SetVideoCurrentTime", CurrentTime.TotalSeconds);
			await JS.InvokeVoidAsync("SetVideoVolume", Volume);
			await JS.InvokeVoidAsync("SetVideoPlaySpeed", PlaySpeed);
		}

		private async Task OnTimeUpdate()
		{
			var seconds = await JS.InvokeAsync<double>("GetVideoCurrentTime");
			CurrentTime = TimeSpan.FromSeconds(seconds);
		}

		private async Task OnEnded()
		{
			if (OnPlayed.HasDelegate)
			{
				await OnPlayed.InvokeAsync();
			}

			_isPlaying = false;
			await JS.InvokeVoidAsync("SetVideoCurrentTime", TimeSpan.Zero.TotalSeconds);
		}

		private async Task OnPlayButtonClick()
		{
			if (!_canPlay)
			{
				return;
			}

			if (_isPlaying)
			{
				await JS.InvokeVoidAsync("PauseVideo");
			}
			else
			{
				await JS.InvokeVoidAsync("PlayVideo");
			}

			_isPlaying = !_isPlaying;
		}

		private async Task OnRewindButtonClick()
		{
			if (CurrentTime.TotalSeconds > 10)
			{
				CurrentTime -= TimeSpan.FromSeconds(10);
			}
			else
			{
				CurrentTime = TimeSpan.Zero;
			}

			await JS.InvokeVoidAsync("SetVideoCurrentTime", CurrentTime.TotalSeconds);
		}

		private async Task OnForwardButtonClick()
		{
			if (Duration.TotalMilliseconds - CurrentTime.TotalSeconds > 10)
			{
				CurrentTime += TimeSpan.FromSeconds(10);
			}
			else
			{
				CurrentTime = Duration;
			}

			await JS.InvokeVoidAsync("SetVideoCurrentTime", CurrentTime.TotalSeconds);
		}

		private async Task OnVolumeButtonClick()
		{
			if (_isMute)
			{
				Volume = _beforeMuteVolume;
			}
			else
			{
				_beforeMuteVolume = Volume;
				Volume = 0;
			}

			_isMute = !_isMute;
			await JS.InvokeVoidAsync("SetVideoVolume", Volume);
		}

		private async Task OnCurrentTimeSliderChanged(ChangeEventArgs e)
		{
			CurrentTime = TimeSpan.FromSeconds(double.Parse(e.Value as string));
			await JS.InvokeVoidAsync("SetVideoCurrentTime", CurrentTime.TotalSeconds);
		}

		private async Task OnVolumeSliderChange(ChangeEventArgs e)
		{
			Volume = double.Parse(e.Value as string);
			await JS.InvokeVoidAsync("SetVideoVolume", Volume);
		}

		private async Task OnSpeedOptionSelect(double value)
		{
			PlaySpeed = value;
			await JS.InvokeVoidAsync("SetVideoPlaySpeed", PlaySpeed);
		}

		private async Task OnFullscreenButtonClick()
		{
			await JS.InvokeVoidAsync("SwitchFullscreen");
		}

		private async Task OnKeyDown(KeyboardEventArgs e)
		{
			switch (e.Key)
			{
				case " ": await OnPlayButtonClick(); break;
				default: break;
			}
		}
	}
}
