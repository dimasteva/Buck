//using Windows.Networking.NetworkOperators;

//using Microsoft.UI.Xaml.Navigation;

namespace Buck
{
    public partial class MainPage : ContentPage
    {
        public Client client;
        bool _isRunning;
        public MainPage(Client client)
        {
            InitializeComponent();
            this.client = client;
            ShowConversationsAndButton();
            InitializeConversationRefresh();
        }
        protected override void OnAppearing()
        {
            System.Diagnostics.Debug.WriteLine("uso u appearing kod maina");
            base.OnAppearing();
            InitializeConversationRefresh();
        }
        private async void InitializeConversationRefresh()
        {
            int secondsForRefresh = 5;
            //System.Diagnostics.Debug.WriteLine("uso u init");
            _isRunning = true;
            while (_isRunning)
            {
                ShowConversationsAndButton();
                System.Diagnostics.Debug.WriteLine("I ovaj jbt         ");

                await Task.Delay(secondsForRefresh * 1000);
            }

        }

        private void ShowConversationsAndButton()
        {
            ChatStackLayout.Clear();
            List<(string SenderId, int UnreadCount)> unreadMessages = Message.GetUnreadOrZeroMessagesBySender(client.Username);

            foreach (var item in unreadMessages)
            {
                AddChat(item.SenderId, item.UnreadCount);
            }
            AddNewButton();
        }

        private async void OnMenuClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Menu", "Cancel", null, "Home", "Settings", "Profile");

            switch (action)
            {
                case "Home":
                    await Navigation.PushAsync(new LoginPage());
                    break;
                case "Settings":
                    await Navigation.PushAsync(new SignUpPage());
                    break;
                case "Profile":
                    await Navigation.PushAsync(new ProfilePage(client));
                    break;
            }
        }

        private void AddChat(string username, int unreadMessages)
        {
            // Kreirajte pravougaonik (Frame) za chat
            var chatFrame = new Frame
            {
                BorderColor = Colors.Gray,
                CornerRadius = 5,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10) // Razmak između chatova
            };

            // Koristimo Grid za postavljanje korisničkog imena i broja nepročitanih poruka
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Dodajte korisničko ime
            var usernameLabel = new Label
            {
                Text = username,
                FontSize = 16,
                Padding = new Thickness(10),
                VerticalOptions = LayoutOptions.Center
            };

            // Kreirajte Frame za broj nepročitanih poruka, ako ih ima
            Frame unreadMessagesFrame = null;
            if (unreadMessages > 0)
            {
                unreadMessagesFrame = new Frame
                {
                    BackgroundColor = Colors.Red,
                    Padding = new Thickness(5),
                    CornerRadius = 15,
                    VerticalOptions = LayoutOptions.Center,
                    MinimumWidthRequest = 40,
                    Content = new Label
                    {
                        Text = unreadMessages.ToString(),
                        FontSize = 16,
                        TextColor = Colors.White,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                };
            }

            // Dodajte komponente u grid
            Grid.SetColumn(usernameLabel, 0);
            Grid.SetRow(usernameLabel, 0);
            grid.Children.Add(usernameLabel);

            if (unreadMessagesFrame != null)
            {
                Grid.SetColumn(unreadMessagesFrame, 1);
                Grid.SetRow(unreadMessagesFrame, 0);
                grid.Children.Add(unreadMessagesFrame);
            }

            // Dodajte grid u Frame
            chatFrame.Content = grid;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                OnChatTapped(username);
            };
            chatFrame.GestureRecognizers.Add(tapGestureRecognizer);

            // Dodajte Frame u StackLayout (ChatStackLayout)
            ChatStackLayout.Children.Add(chatFrame);
        }

        private void OnChatTapped(string username)
        {
            var chatPage = new ChatPage(client, username);
            Navigation.PushAsync(chatPage);
        }

        private void AddNewButton()
        {
            Button newConversationButton = new Button
            {
                Text = "+",
                CornerRadius = 50, // Poluprečnik za zaobljene ivice
                WidthRequest = 50, // Širina dugmeta
                HeightRequest = 50, // Visina dugmeta
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            newConversationButton.Clicked += SendNewMessageAsync;

            ChatStackLayout.Children.Add(newConversationButton);
        }

        private async void SendNewMessageAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewChatPage(client));
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _isRunning = false;
        }
    }

}
