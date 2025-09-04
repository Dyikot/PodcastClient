using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PodcastClient.Components.Controls
{
    public partial class AudioPlayer
    {
        private readonly List<double> _playSpeedOptions = [0.5, 1.0, 1.25, 1.5, 2.0];

        private TimeSpan _currentTime;
        private double _beforeMuteVolume = 0;
        private double _volume;
        private double _playSpeed;
        private bool _canPlay = false;
        private bool _initialized = false;

        public bool IsPlaying { get; private set; } = false;
        public bool IsMute { get; private set; } = false;

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
                if(_currentTime != value)
                {
                    if (CurrentTimeChanged.HasDelegate)
                    {
                        CurrentTimeChanged.InvokeAsync(value);
                    }

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
                if(_volume != value)
                {
                    if (VolumeChanged.HasDelegate)
                    {
                        VolumeChanged.InvokeAsync(value);
                    }

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
                if(_playSpeed != value)
                {
                    if (PlaySpeedChanged.HasDelegate)
                    {
                        PlaySpeedChanged.InvokeAsync(value);
                    }

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
                await JS.InvokeVoidAsync("InitializeAudio");
			}

			await base.OnAfterRenderAsync(firstRender);
		}

		private async Task OnMetadataLoaded()
        {
            _canPlay = true;
            await JS.InvokeVoidAsync("SetCurrentTime", CurrentTime.TotalSeconds);
            await JS.InvokeVoidAsync("SetVolume", Volume);
		    await JS.InvokeVoidAsync("SetPlaySpeed", PlaySpeed);
		}

		private async Task OnTimeUpdate()
        {
            var seconds = await JS.InvokeAsync<double>("GetCurrentTime");
            CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        private async Task OnEnded()
        {
            if(OnPlayed.HasDelegate)
            {
				await OnPlayed.InvokeAsync();
            }
            
            IsPlaying = false;
			await JS.InvokeVoidAsync("SetCurrentTime", TimeSpan.Zero.TotalSeconds);
		}

		private async Task OnPlayButtonClick()
		{
            if(IsPlaying)
            {
				await JS.InvokeVoidAsync("Pause");
			}
            else
            {
				await JS.InvokeVoidAsync("Play");
			}
			
			IsPlaying = !IsPlaying;
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

            await JS.InvokeVoidAsync("SetCurrentTime", CurrentTime.TotalSeconds);
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

			await JS.InvokeVoidAsync("SetCurrentTime", CurrentTime.TotalSeconds);
		}

        private async Task OnVolumeButtonClick()
        {
            if (IsMute)
            {
                Volume = _beforeMuteVolume;
			}
            else
            {
                 _beforeMuteVolume = Volume;
                 Volume = 0;
            }

            IsMute = !IsMute;
			await JS.InvokeVoidAsync("SetVolume", Volume);
		}

        private async Task OnCurrentTimeSliderChanged(ChangeEventArgs e)
        {
			CurrentTime = TimeSpan.FromSeconds(double.Parse((string)e.Value!));
            await JS.InvokeVoidAsync("SetCurrentTime", CurrentTime.TotalSeconds);
		}
            
        private async Task OnVolumeSliderChange(ChangeEventArgs e)
        {
			Volume = double.Parse((string)e.Value!);
			await JS.InvokeVoidAsync("SetVolume", Volume);
		}

        private async Task OnSpeedOptionSelect(double value)
        {
            PlaySpeed = value;
			await JS.InvokeVoidAsync("SetPlaySpeed", PlaySpeed);
		}

        private string GetSpeedOptionLabel(double value)
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
