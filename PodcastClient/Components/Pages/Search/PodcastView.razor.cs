using Microsoft.AspNetCore.Components;
using PodcastClient.Data;

namespace PodcastClient.Components.Pages.Search
{
	public partial class PodcastView
	{
		[Parameter, EditorRequired]
		public Podcast Podcast { get; set; }

		[Parameter]
		public string HrefTarget { get; set; } = "_self";

		public string Href => $"podcast/{Podcast.Id}";
	}
}
