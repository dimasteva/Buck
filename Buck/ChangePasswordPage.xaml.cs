//using Android.Speech;
//using static Java.Util.Jar.Attributes;

namespace Buck;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage()
	{
		InitializeComponent();
	}

	private async void SendEmail(object sender, EventArgs e)
	{
        string email = eEmail.Text;
        
        bool isEmailAlreadyPresent = await Client.IsEmailPresentAsync(email);
        if (!isEmailAlreadyPresent)
        {
            await DisplayAlert("Unable to find account", "Check entered email and try again", "OK");
            return;
        }

        Random random = new Random();
        int confirmationCode = random.Next(100000, 1000000);

        EmailService emailService = new EmailService("smtp.gmail.com", 587, "dimitrije12012007@gmail.com", "pgbg lgre hnce ljsy");
        bool sendEmailResult = await emailService.SendEmailAsync(email, "Find account code", $"Your confirmation code is: {confirmationCode}");

        if (sendEmailResult)
        {
            await DisplayAlert("Email Sent", "Check your email for the confirmation code.", "OK");
        }
        else
        {
            await DisplayAlert("Error", "An error occurred while sending the email. Please try again.", "OK");
            return;
        }

        var processChangePasswordPage = new ProcessChangePasswordPage(confirmationCode, email);
        await Navigation.PushAsync(processChangePasswordPage);
    }
}