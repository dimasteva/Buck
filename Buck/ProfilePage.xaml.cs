namespace Buck;

public partial class ProfilePage : ContentPage
{
	Client client;
	public ProfilePage(Client client)
	{
		InitializeComponent();
		Client client1 = new Client(client.Username, client.Password, client.Name, client.LastName, client.Email);
		this.client = client;
		this.BindingContext = client1;
	}

	private async void SaveInfo(object sender, EventArgs e)
	{
		string username = eUsername.Text;
		string password = ePassword.Text;
		string name = eName.Text;
		string email = eEmail.Text;
		string lastName = eLastName.Text;
		string repeatedPassword = eRepeatPassword.Text;

        var signUpPage = new SignUpPage();
        bool CheckCredentialsResult = await signUpPage.CheckCredentialsAsync(this, username, name, lastName, email, password, repeatedPassword);
        if (!CheckCredentialsResult)
		{

			return;
		}

        client.Username = username;
		client.Password = password;
		client.Name = name;
		client.Email = email;
		client.LastName = lastName;
		bool syncDataResult = await client.SyncDataAsync();
		
		if (syncDataResult)
		{
			await DisplayAlert("Success!", "You have changed your info succesfully!", "OK");
		} else
		{
			await DisplayAlert("Error", "Try again later", "OK");
		}
	}

	private void ShowOrHideRepeatPassword(object sender, EventArgs e)
	{

        if (client.Password != ePassword.Text)
		{
			repeatPasswordLayout.IsVisible = true;
		}
		else
		{
            repeatPasswordLayout.IsVisible = false;
        }
	}
}