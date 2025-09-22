using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;

namespace PodcastClient.Components
{
	public partial class App
	{
		[Inject]
		public IDbContextFactory<ApplicationDbContext> DbContextFactory { get; set; } = default!;

		protected override void OnInitialized()
		{
			using var context = DbContextFactory.CreateDbContext();
			context.Database.EnsureCreated();
		}
	}
}
