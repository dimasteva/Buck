namespace Buck;

public partial class ConfirmationSignUpPage : ContentPage
{
	private int _confirmationCode;
	public ConfirmationSignUpPage(int confirmationCode)
	{
		_confirmationCode = confirmationCode;
		InitializeComponent();
	}

	private void SubmitButtonClicked(object sender, EventArgs e)
	{
		string inputCode = eCode.Text;

		if (inputCode == _confirmationCode.ToString())
		{
			DisplayAlert("Success!", "You have signed up successfully!", "OK");
		} else
		{
			DisplayAlert("Missmatch!", "Confirmation code does not match", "OK");
		}

	}
}