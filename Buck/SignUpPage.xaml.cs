using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Buck;

public partial class SignUpPage : ContentPage
{
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

    public async Task<bool> CheckCredentialsAsync(Page context, string username, string name, string lastName, string email, string password, string repeatedPassword)
    {
        if (!IsValidUsername(username))
        {
            await context.DisplayAlert("Validation Error", "Invalid username. Username must be between 5 and 20 characters long, must start with a letter and can only contain letters and numbers, with no spaces.", "OK");
            return false;
        }

        if (!IsValidName(name))
        {
            await context.DisplayAlert("Validation Error", "Invalid name. It must start with an uppercase letter and contain only alphabetic characters.", "OK");
            return false;
        }

        if (!IsValidLastName(lastName))
        {
            await context.DisplayAlert("Validation Error", "Invalid last name. It must start with an uppercase letter and contain only alphabetic characters.", "OK");
            return false;
        }

        if (!IsValidEmail(email))
        {
            await context.DisplayAlert("Validation Error", "Invalid email format.", "OK");
            return false;
        }

        if (!IsValidPassword(password))
        {
            await context.DisplayAlert("Validation Error", "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and no spaces.", "OK");
            return false;
        }

        if (password != repeatedPassword)
        {
            await context.DisplayAlert("Validation Error", "Passwords do not match.", "OK");
            return false;
        }

        return true;
    }

    private bool IsValidUsername(string username)
    {
        if (username.Length < 5 || username.Length > 20)
        {
            return false;
        }

        if (!char.IsLetter(username[0]))
        {
            return false;
        }

        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
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
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s])[A-Za-z\d@$!%*?&()\-_=+{}[\]#^~`<>.,:;'""|\\]{8,}$";

        if (password.Contains(" "))
        {
            return false;
        }

        return Regex.IsMatch(password, pattern);
    }
    private async void SignUpClicked(object sender, EventArgs e)
    {
        string username = eUsername.Text;
        string name = eName.Text;
        string lastName = eLastName.Text;
        string email = eEmail.Text;
        string password = ePassword.Text;
        string repeatedPassword = eRepeatPassword.Text;

        bool checkCredentialsResult = await CheckCredentialsAsync(this, username, name, lastName, email, password, repeatedPassword);
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

        Client client = new Client(username, password, name, lastName, email);

        var confirmationPage = new ConfirmationSignUpPage(confirmationCode, client);
        await Navigation.PushAsync(confirmationPage);
    }

}