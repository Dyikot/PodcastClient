using Microsoft.EntityFrameworkCore;
using PodcastClient.Data;
using PodcastClient.Resources;
using PodcastClient.Services;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PodcastClient.Components.Pages
{
	public class ChangePasswordModel
	{
		[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
		public string CurrentPassword { get; set; } = string.Empty;

		[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
		[StringLength(maximumLength: 128, MinimumLength = 8, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PasswordValidation")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "FieldRequiredValidation")]
		[Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "ConfirmPasswordValidation")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}

    public partial class Settings
    {
		private string? _errorMessage;
		private bool _passwordChanged = false;

		public CultureInfo[] LanguageOptions { get; } = 
		[
			new CultureInfo("en-US"), 
			new CultureInfo("ru-RU")
		];

		public string[] ThemeOptions { get; } =
		[
			ThemeService.Light,
			ThemeService.Dark
		];

		public ChangePasswordModel ChangePasswordModel { get; set; } = new();

		private async Task OnThemeChanged(string theme)
		{
			await LocalStorage.SetItem(LocalStorageKeys.Theme, theme);
		}

		private string GetLanguageLabel(CultureInfo culture) => culture.Name switch
		{
			"ru-RU" => "Русский",
			"en-US" => "English",
			_ => throw new NotSupportedException()
		};

		private async Task ChangePasswordAsync()
		{
			using var context = DbContextFactory.CreateDbContext();

			var user = await context.Users.FindAsync(User.UserId);

			if(!user!.IsPasswordCorrect(ChangePasswordModel.CurrentPassword))
			{
				_errorMessage = Resource.InvalidCurrentPassword;
				return;
			}

			user.ChangePassword(ChangePasswordModel.Password);
			await context.SaveChangesAsync();
			await CloseChangePasswordDialog();
			_passwordChanged = true;
		}

		private async Task DeleteAccount()
		{
			using var context = DbContextFactory.CreateDbContext();
			await context.Users
				.Where(u => u.Id == User.UserId)
				.ExecuteDeleteAsync();

			NavigationManager.NavigateTo("auth/signout", forceLoad: true);
		}

		private async Task OpenChangePasswordDialog() => await Dialog.Open("changePasswordDialog");
		private async Task CloseChangePasswordDialog() => await Dialog.Close("changePasswordDialog");

		private async Task OpenConfirmDeleteDialog() => await Dialog.Open("confirmDeleteDialog");
		private async Task CloseConfirmDeleteDialog() => await Dialog.Close("confirmDeleteDialog");
	}
}
