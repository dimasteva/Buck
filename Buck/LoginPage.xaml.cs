namespace Buck;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void SignUpClicked(object sender, TappedEventArgs e)
    {
		var signUpPage = new SignUpPage();

		Navigation.InsertPageBefore(signUpPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
    }
}