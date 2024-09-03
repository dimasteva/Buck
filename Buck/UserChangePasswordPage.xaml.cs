using Org.BouncyCastle.Asn1.Cms;

namespace Buck;

public partial class UserChangePasswordPage : ContentPage
{
	string _email;
	public UserChangePasswordPage(string email)
	{
		_email = email;
		InitializeComponent();
	}

	private async void ChangePassword(object sender, EventArgs e)
	{
		string password = ePassword.Text;
		string repeatedPassword = eRepeatPassword.Text;

		var signedUpPage = new SignUpPage();
		if (!signedUpPage.IsValidPassword(password))
		{
            await DisplayAlert("Validation Error", "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and no spaces.", "OK");
			return;
        }
		if (password != repeatedPassword)
		{
			await DisplayAlert("Error", "Passwords do not match", "OK");
			return;
		}

		bool result = await Client.UpdatePasswordAsync(_email, password);
		if (!result)
		{
			await DisplayAlert("Error", "Try again later", "OK");
			return;
		}
        await DisplayAlert("Password changed", "You may login now", "OK");

		var loginPage = new LoginPage();
		Navigation.InsertPageBefore(loginPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
    }
}