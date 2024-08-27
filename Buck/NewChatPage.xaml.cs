namespace Buck;

public partial class NewChatPage : ContentPage
{
	public NewChatPage()
	{
		InitializeComponent();
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
				UserSelected(user);
			};
			userFrame.GestureRecognizers.Add(tapUserFrame);

			UsersStackLayout.Children.Add(userFrame);

        }
		
	}

	private void UserSelected(string user)
	{

	}
}