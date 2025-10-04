using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace PodcastClient.Components.Templates
{
	public static class DataTemplates
	{
		public static void EmptyTemplate(RenderTreeBuilder builder) {}

		public static RenderFragment StringTemplate<T>(T item)
		{
			return builder =>
			{
				builder.AddContent(0, item?.ToString());
			};
		}
	}
}
