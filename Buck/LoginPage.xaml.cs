namespace Buck;

public partial class LoginPage : ContentPage
{
    public static string connectionString = "Server=sql7.freesqldatabase.com;Database=sql7727618;User ID=sql7727618;Password=rm6vECsUfv;Port=3306;";
    public LoginPage()
	{
		InitializeComponent();
	}

    private async void SignUpClickedAsync(object sender, TappedEventArgs e)
    {
		var signUpPage = new SignUpPage();

		Navigation.InsertPageBefore(signUpPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
    }

}