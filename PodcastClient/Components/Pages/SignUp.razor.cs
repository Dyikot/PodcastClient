using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Resources;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PodcastClient.Components.Pages
{
	public partial class SignUp
	{
		public class UserModel
		{
			[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "EmailValidation")]
			public string Email { get; set; } = string.Empty;

			[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[StringLength(maximumLength: 32, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "UserNameValidation")]
			public string UserName { get; set; } = string.Empty;

			[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[StringLength(maximumLength: 128, MinimumLength = 8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordValidation")]
			public string Password { get; set; } = string.Empty;

			[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ConfirmPasswordValidation")]
			public string ConfirmPassword { get; set; } = string.Empty;
		}

		private string? _errorMessage;

		public UserModel User { get; private set; } = new();

		private async Task OnValidSubmit()
		{
			using var context = DbContextFactory.CreateDbContext();

			var exist = await context.Users.AnyAsync(u => u.UserName == User.UserName);

			if (exist)
			{
				_errorMessage = Resource.UserValidationExist;
				return;
			}

			var passwordHasher = new PasswordHasher<IdentityUser>();

			var user = new User
			{
				Email = User.Email,
				UserName = User.UserName,
				HashPassword = passwordHasher.HashPassword(null!, User.Password)
			};

			context.Users.Add(user);
			await context.SaveChangesAsync();

			NavigationManager.NavigateTo("/signin");
		}
	}
}
