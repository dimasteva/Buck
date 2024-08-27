namespace Buck;

public partial class ChatPage : ContentPage
{
	string _reciever;
	Client _client;
	public ChatPage(Client client, string receiver)
	{
		InitializeComponent();
		_reciever = receiver;
		_client = client;
		CreateReceiverLabel();
	}

	private async void SendMessageAsync(object sender, EventArgs e)
	{

	}

	private void CreateReceiverLabel()
	{
        Label lReciever = new Label
        {
            Text = _reciever,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        Grid.SetRow(lReciever, 0);
        Grid.SetColumn(lReciever, 0);

        gridName.Children.Add(lReciever);
    }
}