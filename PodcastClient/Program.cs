using Microsoft.AspNetCore.Authentication.Cookies;
using PodcastClient.Components;
using PodcastClient.Data;
using PodcastClient.Extensions;
using PodcastClient.Services;
using System.Threading.Tasks;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddRazorComponents().AddInteractiveServerComponents();
		builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.LoginPath = "/signin";
				options.LogoutPath = "/";
			});
		builder.Services.AddAuthorization();
		builder.Services.AddCascadingAuthenticationState();
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
		builder.Services.AddScoped<DialogService>();
		builder.Services.AddSingleton<CategoriesService>();
		builder.Services.AddSingleton<PodcastRssFetcher>();

		var app = builder.Build();

		app.UseHttpsRedirection();
		app.UseStaticFiles();
		app.UseAntiforgery();
		app.UseAuthentication();
		app.UseAuthorization();
		app.UseRequestLocalization(options =>
		{
			string[] cultures = ["ru-RU", "en-US"];
			options.AddSupportedCultures(cultures)
				   .AddSupportedUICultures(cultures)
				   .SetDefaultCulture(cultures[0]);
		});
		app.MapControllers();
		app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
		app.SeedDatabase();

		app.Run();
	}
}