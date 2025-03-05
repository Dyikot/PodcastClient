using PodcastClient.Components;
using PodcastClient.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddSingleton<ThemeService>();
builder.Services.AddSingleton<IPodcastService, PodcastService>();
builder.Services.AddSingleton<IMediaService, MediaService>();
builder.Services.AddTransient<IDateFormatter, DateFormatter>();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseRequestLocalization(options =>
{
	options.AddSupportedCultures(["ru-RU", "en-US"]);
	options.AddSupportedUICultures(["ru-RU", "en-US"]);
	options.SetDefaultCulture("ru-RU");
});
app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();