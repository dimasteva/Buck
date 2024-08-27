namespace Buck;

public partial class ChatPage : ContentPage
{
	private string _receiver;
	Client _client;
	public ChatPage(Client client, string receiver)
	{
		InitializeComponent();
		_receiver = receiver;
		_client = client;
		CreateReceiverLabel();
	}

	private async void SendMessageAsync(object sender, EventArgs e)
	{
		string content = eMessage.Text;
		bool sendMessageResult = await _client.SendMessageAsync(content, _receiver);
		if (!sendMessageResult)
		{
			await DisplayAlert("Error", "Message could not be send, try again later", "OK");
			return;
		}
		ShowSentMessageAsync(content);
		eMessage.Text = "";
	}

	private async void ShowSentMessageAsync(string content)
	{
		var messageFrame = new Frame();
        var messageLabel = new Label
        {
            Text = content,
            HorizontalOptions = LayoutOptions.End
        };
        messageFrame.Content = messageLabel;

		MessagesStackLayout.Children.Add(messageFrame);
	}

	private void CreateReceiverLabel()
	{
        Label lReciever = new Label
        {
            Text = _receiver,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        //Grid.SetRow(lReciever, 0);
        //Grid.SetColumn(lReciever, 0);

        gridName.Children.Add(lReciever);
    }
}