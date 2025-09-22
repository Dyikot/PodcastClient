using Microsoft.AspNetCore.Components;

namespace PodcastClient.Components.Templates
{
	public static class DataTemplates
	{
		public static RenderFragment StringTemplate<T>(T item)
		{
			return builder =>
			{
				builder.AddContent(0, item?.ToString());
			};
		}
	}
}
