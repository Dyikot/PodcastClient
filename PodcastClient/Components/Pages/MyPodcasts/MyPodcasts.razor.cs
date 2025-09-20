using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Models;

namespace PodcastClient.Components.Pages.MyPodcasts
{
    public partial class MyPodcasts
    {
		public List<Podcast> Podcasts { get; set; } = [];
		public string RssFeed { get; set; } = string.Empty;
        private bool _isSubscribePanelVisible = false;
		private bool _isMouseOverSubscribePanel = false;
		private bool _wasSubscribeButtonClicked = false;
		private InputText? _rssInputFeed;

		protected override async Task OnInitializedAsync()
		{
			using var context = DbContextFactory.CreateDbContext();

			var user = await context.Users
				.AsNoTracking()
				.Include(u => u.Podcasts)
				.SingleAsync(u => u.Id == UserContext.UserId);

			Podcasts = user.Podcasts;
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
            Navigator.NavigateTo($"/podcast/{podcast.Id}");
        }

		private async Task OnSubmitButtonClick()
		{
			_isSubscribePanelVisible = false;

			if (!string.IsNullOrEmpty(RssFeed))
			{
				try
				{
					var rss = new Uri(RssFeed);
					var podcast = await PodcastsService.FindAsync(p => p.Rss == rss);

					if(podcast == null)
					{
						podcast = await PodcastRssFetcher.GetPodcastAsync(rss);
						if(podcast != null)
						{
							await PodcastsService.AddPodcastAsync(podcast);
						}
					}

					if (podcast != null)
					{
						await UserContext.AddPodcastAsync(podcast);
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
