using Microsoft.Extensions.Localization;
using PodcastClient.Resources;

namespace PodcastClient.Services
{
	public class CategoriesService
	{
		public CategoriesService(IStringLocalizer<Resource> localizer)
		{
			Arts = new CategoryViewModel(localizer, "Arts", "category/arts", "Home/Palette.svg");
			Business = new CategoryViewModel(localizer, "Business", "category/business", "Home/Business.svg");
			Comedy = new CategoryViewModel(localizer, "Comedy", "category/comedy", "Home/Comedy.svg");
			Education = new CategoryViewModel(localizer, "Education", "category/education", "Home/School.svg");
			Fiction = new CategoryViewModel(localizer, "Fiction", "category/fiction", "Home/Book.svg");
			Government = new CategoryViewModel(localizer, "Government", "category/government", "Home/Government.svg");
			History = new CategoryViewModel(localizer, "History", "category/history", "Home/Book2.svg");
			HealthAndFitness = new CategoryViewModel(localizer, "HealthAndFitness", "category/health-fitness", "Home/Health.svg");
			KidsAndFamily = new CategoryViewModel(localizer, "KidsAndFamily", "category/kids-family", "Home/Family.svg");
			Leisure = new CategoryViewModel(localizer, "Leisure", "category/leisure", "Home/Leisure.svg");
			Music = new CategoryViewModel(localizer, "Music", "category/music", "Home/Music.svg");
			News = new CategoryViewModel(localizer, "News", "category/news", "Home/News.svg");
			ReligionAndSpirituality = new CategoryViewModel(localizer, "ReligionAndSpirituality", "category/religion-spirituality", "Home/Religion.svg");
			Science = new CategoryViewModel(localizer, "Science", "category/science", "Home/Science.svg");
			SocietyAndCulture = new CategoryViewModel(localizer, "SocietyAndCulture", "category/society-culture", "Home/Building.svg");
			Sports = new CategoryViewModel(localizer, "Sports", "category/sports", "Home/Sports.svg");
			Technology = new CategoryViewModel(localizer, "Technology", "category/technology", "Home/Technology.svg");
			TrueCrime = new CategoryViewModel(localizer, "TrueCrime", "category/true-crime", "Home/Police.svg");
			TvAndFilm = new CategoryViewModel(localizer, "TvAndFilm", "category/tv-film", "Home/TV.svg");
		}

		public CategoryViewModel Arts { get; init; }
		public CategoryViewModel Business { get; init; }
		public CategoryViewModel Comedy { get; init; }
		public CategoryViewModel Education { get; init; }
		public CategoryViewModel Fiction { get; init; }
		public CategoryViewModel Government { get; init; }
		public CategoryViewModel History { get; init; }
		public CategoryViewModel HealthAndFitness { get; init; }
		public CategoryViewModel KidsAndFamily { get; init; }
		public CategoryViewModel Leisure { get; init; }
		public CategoryViewModel Music { get; init; }
		public CategoryViewModel News { get; init; }
		public CategoryViewModel ReligionAndSpirituality { get; init; }
		public CategoryViewModel Science { get; init; }
		public CategoryViewModel SocietyAndCulture { get; init; }
		public CategoryViewModel Sports { get; init; }
		public CategoryViewModel Technology { get; init; }
		public CategoryViewModel TrueCrime { get; init; }
		public CategoryViewModel TvAndFilm { get; init; }

		public IEnumerable<CategoryViewModel> GetAllCategories()
		{
			return
			[
				Arts, Business, Comedy, Education, Fiction, Government, History,
				HealthAndFitness, KidsAndFamily, Leisure, Music, News,
				ReligionAndSpirituality, Science, SocietyAndCulture, Sports,
				Technology, TrueCrime, TvAndFilm
			];
		}
	}

	public class CategoryViewModel
	{
		private readonly IStringLocalizer _localizer;
		private readonly string _name;

		public CategoryViewModel(IStringLocalizer localizer, string name, 
								 string uri, string imageSorce)
		{
			_localizer = localizer;
			_name = name;
			Uri = uri;
			ImageSource = imageSorce;
		}

		public string LocalizedName => _localizer[_name];
		public string Uri { get; set; }
		public string ImageSource { get; set; }
	}
}
