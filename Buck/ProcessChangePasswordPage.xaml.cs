namespace Buck;

public partial class ProcessChangePasswordPage : ContentPage
{
	int _code;
	string _email;
	public ProcessChangePasswordPage(int code, string email)
	{
		InitializeComponent();
		_code = code;
		_email = email;
	}

	private async void SubmitButtonClicked(object sender, EventArgs e)
	{
        string inputCode = eCode.Text;


        if (inputCode != _code.ToString())
        {
            await DisplayAlert("Missmatch!", "Confirmation code does not match", "OK");
            return;
        }

		var userChangePasswordPage = new UserChangePasswordPage(_email);
		await Navigation.PushAsync(userChangePasswordPage);
    }
}