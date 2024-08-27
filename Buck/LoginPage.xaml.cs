namespace Buck;

public partial class LoginPage : ContentPage
{
	private string _username, _password;

    public static string connectionString = "Server=sql7.freesqldatabase.com;Database=sql7727618;User ID=sql7727618;Password=rm6vECsUfv;Port=3306;";
    public LoginPage()
	{
		InitializeComponent();
	}

	private async void LoginClickedAsync(object sender, EventArgs e)
	{
		_username = eUsername.Text;
		_password = ePassword.Text;

		bool doesAccountExistResult = await Client.DoesAccountExistAsync(_username, _password);

		if (doesAccountExistResult)
		{
			await DisplayAlert("Success!", "You have been logged in", "OK");
		} else
		{
			await DisplayAlert("Invalid credentials", "Check your username and password and try again", "OK");
			return;
		}

        var mainPage = new MainPage();
        Navigation.InsertPageBefore(mainPage, Navigation.NavigationStack.FirstOrDefault());
        await Navigation.PopToRootAsync();
    }

    private async void SignUpClickedAsync(object sender, TappedEventArgs e)
    {
		var signUpPage = new SignUpPage();

		Navigation.InsertPageBefore(signUpPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
    }

}