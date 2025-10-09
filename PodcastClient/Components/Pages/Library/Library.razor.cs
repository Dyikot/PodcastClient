using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PodcastClient.Data;
using PodcastClient.Resources;
using System.ComponentModel.DataAnnotations;

namespace PodcastClient.Components.Pages.Library
{
	public class RssFeedModel
	{
		[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
		[Url(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidUrl")]
		public string Url { get; set; } = string.Empty;
	}

    public partial class Library
    {
		private EditContext? _editContext;

		public List<Podcast> Podcasts { get; set; } = [];
		public RssFeedModel RssFeed { get; set; } = new();
		public bool IsAddingPodcast { get; private set; } = false;

		protected override async Task OnInitializedAsync()
		{
			_editContext = new(RssFeed);

			using var context = DbContextFactory.CreateDbContext();

			var user = await context.Users
				.AsNoTracking()
				.Include(u => u.Podcasts)
				.SingleAsync(u => u.Id == UserContext.UserId);

			Podcasts = user.Podcasts;
		}

		private async Task OpenRssDialog()
		{
			await JS.InvokeVoidAsync("OpenDialog", "rssDialog");
		}

		private async Task CloseRssDialog()
		{
			RssFeed.Url = string.Empty;
			_editContext = new(RssFeed);
			await JS.InvokeVoidAsync("CloseDialog", "rssDialog");
		}

		private async Task OnRssFeedSubmit()
		{
			var rss = new Uri(RssFeed.Url);

			IsAddingPodcast = true;
			await CloseRssDialog();
			await TryAddPodcast(rss);
			IsAddingPodcast = false;
		}

		private async Task TryAddPodcast(Uri rss)
		{
			try
			{				
				if (Podcasts.Any(p => p.Rss == rss))
				{
					return;
				}

				var podcast = await PodcastsService.FindAsync(p => p.Rss == rss);

				if (podcast == null)
				{
					podcast = await PodcastRssFetcher.GetPodcastAsync(rss);
					if (podcast != null)
					{
						await PodcastsService.AddAsync(podcast);
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
		}
	}
}
