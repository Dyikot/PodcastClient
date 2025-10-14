using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PodcastClient.Resources;
using System.ComponentModel.DataAnnotations;

namespace PodcastClient.Components.Pages
{
	public partial class SignIn
	{
		public class UserModel
		{
			[Required(ErrorMessageResourceType =typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[StringLength(maximumLength: 32, MinimumLength = 3, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "UserNameValidation")]
			public string UserName { get; set; } = string.Empty;

			[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
			[StringLength(maximumLength: 128, MinimumLength = 8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordValidation")]
			public string Password { get; set; } = string.Empty;
		}

		private string? _errorMessage;

		public UserModel User { get; private set; } = new();

		private async Task OnValidSubmit()
		{
			using var context = DbContextFactory.CreateDbContext();

			var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == User.UserName);
			if(user == null)
			{
				_errorMessage = Resource.InvalidUsernameOrPassword;
				return;
			}

			if (!user.IsPasswordCorrect(User.Password))
			{
				_errorMessage = Resource.InvalidUsernameOrPassword;
				return;
			}
			
			NavigationManager.NavigateTo(
				uri: $"/Auth/SignIn?userId={user.Id}",
				forceLoad: true);
		}
	}
}
