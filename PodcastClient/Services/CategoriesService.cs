using PodcastClient.Data;

namespace PodcastClient.Services
{
	public class CategoriesService
	{
		public CategoriesService(IConfiguration configuration)
		{
			Categories = configuration
				.GetSection("PodcastCategories")
				.Get<List<CategoryInfo>>()!;
		}

		public List<CategoryInfo> Categories { get; init; }
	}	
}
