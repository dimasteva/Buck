namespace Buck;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

	private async void LoginClicked(object sender, EventArgs e)
	{
		var loginPage = new LoginPage();

		Navigation.InsertPageBefore(loginPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
	}
}