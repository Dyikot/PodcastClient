using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Controls
{
	public abstract class PlayerBase : ComponentBase
	{
		public const string TimeFormat = "hh':'mm':'ss";
		public readonly List<double> PlaySpeedOptions = [0.5, 1.0, 1.25, 1.5, 2.0];

		protected TimeSpan _currentTime;
		protected double _beforeMuteVolume = 0;
		protected double _volume;
		protected double _playSpeed;
		private bool _hasMuteButtonPressed = false;
		private bool _initialized = false;

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
				if (_currentTime != value)
				{
					if (CurrentTimeChanged.HasDelegate)
						CurrentTimeChanged.InvokeAsync(value);

					_currentTime = value;
				}
			}
		}

		[Parameter, EditorRequired]
		public double Volume
		{
			get => _volume;
			set
			{
				if (_volume != value)
				{
					if (VolumeChanged.HasDelegate)
						VolumeChanged.InvokeAsync(value);

					_volume = value;
				}
			}
		}

		[Parameter, EditorRequired]
		public double PlaySpeed
		{
			get => _playSpeed;
			set
			{
				if (_playSpeed != value)
				{
					if (PlaySpeedChanged.HasDelegate)
						PlaySpeedChanged.InvokeAsync(value);

					_playSpeed = value;
				}
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

		public bool CanPlay { get; protected set; } = false;
		public bool IsPlaying { get; protected set; } = false;
		public bool IsMute => _volume == 0;

		public double VolumePersentage => Volume / 1 * 100;
		public double DurationPersentage => CurrentTime / Duration * 100;

		public override async Task SetParametersAsync(ParameterView parameters)
		{
			if (!_initialized)
			{
				_initialized = true;
				await base.SetParametersAsync(parameters);
			}
		}

		protected abstract Task InitializeControlAsync();
		protected abstract Task<double> GetCurrentTimeAsync();
		protected abstract Task SetCurrentTimeAsync(TimeSpan value);
		protected abstract Task SetVolumeAsync(double value);
		protected abstract Task SetPlaySpeedAsync(double value);
		protected abstract Task PlayAsync();
		protected abstract Task PauseAsync();

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await InitializeControlAsync();
			}

			await base.OnAfterRenderAsync(firstRender);
		}		

		protected async Task OnTimeUpdate()
		{
			var seconds = await GetCurrentTimeAsync();
			CurrentTime = TimeSpan.FromSeconds(seconds);
		}

		protected async Task OnMetadataLoaded()
		{
			CanPlay = true;
			await SetCurrentTimeAsync(CurrentTime);
			await SetVolumeAsync(Volume);
			await SetPlaySpeedAsync(PlaySpeed);
		}

		protected async Task OnEnded()
		{
			if (OnPlayed.HasDelegate)
			{
				await OnPlayed.InvokeAsync();
			}

			IsPlaying = false;
			await SetCurrentTimeAsync(TimeSpan.Zero);
		}

		protected async Task OnPlayButtonClick()
		{
			if (!CanPlay)
			{
				return;
			}

			if (IsPlaying)
			{
				await PauseAsync();
			}
			else
			{
				await PlayAsync();
			}

			IsPlaying = !IsPlaying;
		}

		protected async Task OnRewindButtonClick()
		{
			if (CurrentTime.TotalSeconds > 10)
			{
				CurrentTime -= TimeSpan.FromSeconds(10);
			}
			else
			{
				CurrentTime = TimeSpan.Zero;
			}

			await SetCurrentTimeAsync(CurrentTime);
		}

		protected async Task OnForwardButtonClick()
		{
			if (Duration.TotalMilliseconds - CurrentTime.TotalSeconds > 10)
			{
				CurrentTime += TimeSpan.FromSeconds(10);
			}
			else
			{
				CurrentTime = Duration;
			}

			await SetCurrentTimeAsync(CurrentTime);
		}

		protected async Task OnMuteButtonClick()
		{
			if (_hasMuteButtonPressed)
			{
				Volume = _beforeMuteVolume;
			}
			else
			{
				_beforeMuteVolume = Volume;
				Volume = 0;
			}

			_hasMuteButtonPressed = !_hasMuteButtonPressed;
			await SetVolumeAsync(Volume);
		}

		protected async Task OnCurrentTimeSliderChanged(ChangeEventArgs e)
		{
			CurrentTime = TimeSpan.FromSeconds(double.Parse((string)e.Value!));
			await SetCurrentTimeAsync(CurrentTime);
		}

		protected async Task OnVolumeSliderChanged(ChangeEventArgs e)
		{
			Volume = double.Parse((string)e.Value!);
			await SetVolumeAsync(Volume);
		}

		protected async Task OnSpeedOptionSelect(double value)
		{
			PlaySpeed = value;
			await SetVolumeAsync(PlaySpeed);
		}

		protected string GetSpeedOptionLabel(double value)
		{
			return value switch
			{
				0.5 => "0.5x",
				1.0 => "1x",
				1.25 => "1.25x",
				1.5 => "1.5x",
				2.0 => "2x",
				_ => $"{value}x"
			};
		}
	}
}
