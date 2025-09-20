using PodcastClient.Components;
using PodcastClient.Models;
using PodcastClient.Services;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Services.AddRazorComponents().AddInteractiveServerComponents();
		builder.Services.AddDbContextFactory<ApplicationDbContext>();
		builder.Services.AddHttpClient();
		builder.Services.AddLocalization();
		builder.Services.AddControllers();
		builder.Services.AddScoped<UserContext>();
		builder.Services.AddScoped<PodcastsService>();
		builder.Services.AddScoped<LocalStorage>();
		builder.Services.AddScoped<CultureService>();
		builder.Services.AddScoped<ThemeService>();
		builder.Services.AddScoped<MediaService>();
		builder.Services.AddSingleton<PodcastRssFetcher>();

		var app = builder.Build();
		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseAntiforgery();
		app.UseRequestLocalization(options =>
		{
			string[] cultures = ["ru-RU", "en-US"];
			options.AddSupportedCultures(cultures)
				   .AddSupportedUICultures(cultures)
				   .SetDefaultCulture(cultures[0]);
		});
		app.MapControllers();
		app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

		app.Run();
	}
}