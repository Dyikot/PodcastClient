using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PodcastClient.Controlles
{
	[Route("[controller]/[action]")]
	public class AuthController : Controller
	{
		public IActionResult Index() => View();

		public async Task<IActionResult> SignIn(string userId)
		{
			var claim = new Claim(ClaimTypes.NameIdentifier, userId);
			var identiry = new ClaimsIdentity([claim], "Cookies");
			var principal = new ClaimsPrincipal(identiry);

			await HttpContext.SignInAsync("Cookies", principal, new AuthenticationProperties
			{
				IsPersistent = true,
				ExpiresUtc = DateTime.UtcNow.AddDays(30)
			});
			return LocalRedirect("/");
		}

		public new async Task<IActionResult> SignOut()
		{
			await HttpContext.SignOutAsync("Cookies");
			return LocalRedirect("/");
		}
	}
}
