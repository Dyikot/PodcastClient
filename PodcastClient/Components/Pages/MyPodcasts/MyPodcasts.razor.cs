using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class MyPodcasts
    {
		public IList<Podcast> Podcasts { get; set; } = [];
		public string RssFeed { get; set; } = string.Empty;
        private bool _isSubscribePanelVisible = false;
		private bool _isMouseOverSubscribePanel = false;
		private bool _wasSubscribeButtonClicked = false;
		private InputText? _rssInputFeed;

		protected override void OnInitialized()
        {
			base.OnInitialized();
			Podcasts = PodcastCollection;

			if (Podcasts.Count == 0)
			{
				var httpClient = HttpClientFactory.CreateClient();
				var rss = httpClient.GetStringAsync("https://feeds.simplecast.com/h18ZIZD_").Result;
				var podcast = Podcast.TryParse(rss);
				Podcasts.Add(podcast!);

				rss = httpClient.GetStringAsync("https://feeds.twit.tv/twit_video_hd.xml").Result;
				podcast = Podcast.TryParse(rss);
				Podcasts.Add(podcast!);
			}
		}


		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (_wasSubscribeButtonClicked)
			{
				_wasSubscribeButtonClicked = false;
				await _rssInputFeed!.Element!.Value.FocusAsync();
			}
		}

		private void OnSubscribeClick()
		{
			_isSubscribePanelVisible = true;
			_wasSubscribeButtonClicked = true;
		}

		private void OnSubscribePanelFocusOut()
		{
			if (!_isMouseOverSubscribePanel && !_wasSubscribeButtonClicked)
			{
				_isSubscribePanelVisible = false;
			}
		}

		private void OnSubscribePanelMouseEnter() => _isMouseOverSubscribePanel = true;
		private void OnSubscribePanelMouseLeave() => _isMouseOverSubscribePanel = false;

		private void OnPodcastClick(Podcast podcast)
        {
            Navigator.NavigateTo($"/my_podcasts/{podcast.Name}");
        }

		private async Task OnSubmitButtonClick()
		{
			_isSubscribePanelVisible = false;

			if (RssFeed != string.Empty)
			{
				try
				{
					var rss = await HttpClientFactory.CreateClient().GetStringAsync(RssFeed);
					var podcast = await Task.Run(() => Podcast.TryParse(rss));

					if (podcast != null)
					{
						Podcasts.Add(podcast);
					}
				}
				catch (Exception ex)
				{
					Logger.LogError(ex.Message);
				}

				RssFeed = string.Empty;
            }
		}
	}
}
