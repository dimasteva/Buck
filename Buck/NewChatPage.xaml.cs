namespace Buck;

public partial class NewChatPage : ContentPage
{
	Client client;
	public NewChatPage(Client client)
	{
		InitializeComponent();
		this.client = client;
	}

	private async void SearchUsersAsync(object sender, EventArgs e)
	{
		string startsWith = eSearch.Text;

		List<string> users = await Client.SearchClientsAsync(startsWith);
        ShowUsersAsync(users);
	}

	private async void ShowUsersAsync(List<string> users)
	{
        UsersStackLayout.Children.Clear();

        foreach (string user in users)
		{
			if (user == client.Username)
			{
				continue;
			}

            var userFrame = new Frame
            {
                //BorderColor = Colors.Gray,
                //CornerRadius = 5,
                //Padding = new Thickness(10),
                //Margin = new Thickness(0, 0, 0, 10),
				//HorizontalOptions = LayoutOptions.Center,
				//WidthRequest = -1
            };
            var userLabel = new Label
            {
                Text = user,
                //VerticalOptions = LayoutOptions.Center,
                //HorizontalOptions = LayoutOptions.Center
            };

            userFrame.Content = userLabel;

			var tapUserFrame = new TapGestureRecognizer();
			tapUserFrame.Tapped += (s, e) =>
			{
				UserSelectedAsync(user);
			};
			userFrame.GestureRecognizers.Add(tapUserFrame);

			UsersStackLayout.Children.Add(userFrame);

        }
		
	}

	private async void UserSelectedAsync(string user)
	{
        await Navigation.PushAsync(new ChatPage(client, user));
        Navigation.RemovePage(this);
    }
}