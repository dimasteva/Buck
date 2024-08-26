using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Buck;

public partial class SignUpPage : ContentPage
{
	private string name, lastName, email, password, repeatedPassword;
	public SignUpPage()
	{
		InitializeComponent();
	}

	private async void LoginClickedAsync(object sender, EventArgs e)
	{
		var loginPage = new LoginPage();

		Navigation.InsertPageBefore(loginPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
	}

    private async Task<bool> CheckCredentialsAsync()
    {
        if (!IsValidName(name))
        {
            await DisplayAlert("Validation Error", "Invalid name. It must start with an uppercase letter and contain only alphabetic characters.", "OK");
            return false;
        }

        if (!IsValidLastName(lastName))
        {
            await DisplayAlert("Validation Error", "Invalid last name. It must start with an uppercase letter and contain only alphabetic characters.", "OK");
            return false;
        }

        if (!IsValidEmail(email))
        {
            await DisplayAlert("Validation Error", "Invalid email format.", "OK");
            return false;
        }

        if (!IsValidPassword(password))
        {
            await DisplayAlert("Validation Error", "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and no spaces.", "OK");
            return false;
        }

        if (password != repeatedPassword)
        {
            await DisplayAlert("Validation Error", "Passwords do not match.", "OK");
            return false;
        }

        return true;
    }

    private bool IsValidName(string name)
    {
        if (!char.IsUpper(name[0]))
        {
            return false;
        }

        string pattern = @"^[A-Za-z]+$";
        return Regex.IsMatch(name, pattern);
    }

    private bool IsValidLastName(string lastName)
    {
        if (!char.IsUpper(lastName[0]))
        {
            return false;
        }

        string pattern = @"^[A-Za-z]+$";
        return Regex.IsMatch(lastName, pattern);
    }

    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    private bool IsValidPassword(string password)
    {
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

        if (password.Contains(" "))
        {
            return false;
        }

        return Regex.IsMatch(password, pattern);
    }
    private async void SignUpClicked(object sender, EventArgs e)
    {
        name = eName.Text;
        lastName = eLastName.Text;
        email = eEmail.Text;
        password = ePassword.Text;
        repeatedPassword = eRepeatPassword.Text;

        bool checkCredentialsResult = await CheckCredentialsAsync();
        if (!checkCredentialsResult)
        {
            return;
        }

        Random random = new Random();
        int confirmationCode = random.Next(100000, 1000000);

        EmailService emailService = new EmailService("smtp.gmail.com", 587, "dimitrije12012007@gmail.com", "pgbg lgre hnce ljsy");
        bool sendEmailResult = await emailService.SendEmailAsync(email, "Sign up confirmation", $"Your confirmation code is: {confirmationCode}");

        if (sendEmailResult)
        {
            await DisplayAlert("Email Sent", "Check your email for the confirmation code.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "An error occurred while sending the email. Please try again.", "OK");
            return;
        }

        var confirmationPage = new ConfirmationSignUpPage(confirmationCode);
        await Navigation.PushAsync(confirmationPage);
    }

}