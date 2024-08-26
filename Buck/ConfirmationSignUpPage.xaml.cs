namespace Buck;

public partial class ConfirmationSignUpPage : ContentPage
{
	private int _confirmationCode;
	private Client _client;
	public ConfirmationSignUpPage(int confirmationCode, Client client)
	{
		_confirmationCode = confirmationCode;
		_client = client;
		InitializeComponent();
	}

	private async void SubmitButtonClicked(object sender, EventArgs e)
	{
		await DisplayAlert("DA", "DA", "DA");

		string inputCode = eCode.Text;


        if (inputCode != _confirmationCode.ToString())
		{
            await DisplayAlert("Missmatch!", "Confirmation code does not match", "OK");
            return;
        }

		bool createAccountResult = await _client.CreateAccountAsync();

		if (createAccountResult)
		{
            await DisplayAlert("Success!", "You have signed up successfully!", "OK");
        } else
		{
			await DisplayAlert("Error occured", "Try again later", "OK");
			return;
		}

		var mainPage = new MainPage();
		Navigation.InsertPageBefore(mainPage, Navigation.NavigationStack.FirstOrDefault());
		await Navigation.PopToRootAsync();
	}
}