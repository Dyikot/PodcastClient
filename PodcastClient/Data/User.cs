using Microsoft.AspNetCore.Identity;

namespace PodcastClient.Data
{
	public class User
	{
		public int Id { get; set; }

		public string Email { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string HashPassword { get; set; } = string.Empty;
		public DateTime LastUpdateChecked { get; set; }

		public List<Podcast> Podcasts { get; set; }
		public List<UserEpisode> Episodes { get; set; }

		public bool IsPasswordCorrect(string password)
		{
			var passwordHasher = new PasswordHasher<IdentityUser>();
			var result = passwordHasher.VerifyHashedPassword(null!, HashPassword, password);
			return result != PasswordVerificationResult.Failed;
		}

		public void ChangePassword(string newPassword)
		{
			var passwordHasher = new PasswordHasher<IdentityUser>();
			HashPassword = passwordHasher.HashPassword(null!, newPassword);
		}
	}
}
