using Microsoft.Maui.Controls.Shapes;
using System.Text;

namespace Buck;

public partial class ChatPage : ContentPage
{
	private string _receiver;
	Client _client;
    bool _isRunning;
	public ChatPage(Client client, string receiver)
	{
		InitializeComponent();
		_receiver = receiver;
		_client = client;
		CreateReceiverLabel();
        int checkDatabaseIntervalSeconds = 5;
        StartChecking(checkDatabaseIntervalSeconds);
	}

    private async void StartChecking(int seconds)
    {
        _isRunning = true;
        while(_isRunning)
        {
            CheckNewMessages();

            await Task.Delay(seconds * 1000);
        }
    }

    private async void CheckNewMessages()
    {
        var unreadMessages = await Message.GetUnreadMessagesAsync(_receiver, _client.Username);

        foreach (var message in unreadMessages)
        {
            ShowMessageAsync(message.Content, "received");
        }
        await Message.MarkMessagesAsReadAsync(_receiver, _client.Username);
    }

	private async void SendMessageAsync(object sender, EventArgs e)
	{
		string content = eMessage.Text;
        var message = new Message(_client.Username, _receiver, content, DateTime.UtcNow);
		bool sendMessageResult = await message.SendMessageAsync();
		if (!sendMessageResult)
		{
			await DisplayAlert("Error", "Message could not be send, try again later", "OK");
			return;
		}
		ShowMessageAsync(content);
		eMessage.Text = "";
	}

    private void ShowMessageAsync(string content, string type = "sent")
    {
        int maxCharMessageWidth = 80;
        AddNewLineCharacterInterval(ref content, maxCharMessageWidth);

        var messageLabel = new Label
        {
            Text = content,
            HorizontalOptions = LayoutOptions.End, // Poravnanje na pocetak da zauzme samo potrebnu sirinu
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Black,                // Boja teksta
            FontSize = 14,                            // Velicina fonta
            Padding = new Thickness(10),              // Unutrasnje poravnavanje
        };

        var messageBorder = new Border
        {
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 10 // Zaobljeni uglovi
            },
            BackgroundColor = type == "sent" ? Colors.Blue : Colors.LightGray,      // Pozadinska boja
            StrokeThickness = 0,                     // Bez obruba
            HorizontalOptions = type == "sent" ? LayoutOptions.End : LayoutOptions.Start,   // Poravnanje desno
            Margin = new Thickness(0, 0, 0, 4),
            Content = messageLabel                   // Sadrzaj unutar Border-a je Label
        };

        MessagesStackLayout.Children.Add(messageBorder);
    }

    private void AddNewLineCharacterInterval(ref string content, int maxCharMessageWidth)
    {
        if (maxCharMessageWidth <= 0)
        {
            throw new ArgumentException("maxCharMessageWidth must be greater than 0.");
        }

        // Razdvajanje sadr�aja na re?i
        string[] words = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        StringBuilder sb = new StringBuilder();
        int currentLineLength = 0;

        foreach (string word in words)
        {
            if (currentLineLength + word.Length + (sb.Length > 0 ? 1 : 0) > maxCharMessageWidth)
            {
                // Dodaj novi red
                sb.Append('\n');
                currentLineLength = 0;
            }

            // Dodaj re? i razmak
            if (currentLineLength > 0)
            {
                sb.Append(' ');
            }
            sb.Append(word);
            currentLineLength += word.Length + (currentLineLength > 0 ? 1 : 0);
        }

        content = sb.ToString();
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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isRunning = false; // Zaustavlja petlju kada Page nije vidljiv
    }
}