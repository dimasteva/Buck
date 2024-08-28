using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector; // MySqlConnector namespace

namespace Buck
{
    internal class Message
    {
        string _sender, _receiver, _content;

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        DateTime _timestamp;

        public Message(string sender, string receiver, string content, DateTime timestamp)
        {
            _sender = sender;
            _receiver = receiver;
            _content = content;
            _timestamp = timestamp;
        }
        public async Task<bool> SendMessageAsync()
        {
            string query = "INSERT INTO Messages (senderId, receiverId, content, timestamp) VALUES (@SenderId, @ReceiverId, @Content, @Timestamp);";
            try
            {
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Dodajte parametre u SQL upit
                        command.Parameters.AddWithValue("@SenderId", _sender);
                        command.Parameters.AddWithValue("@ReceiverId", _receiver);
                        command.Parameters.AddWithValue("@Content", _content);
                        command.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);

                        // Izvršite upit
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static async Task<List<(string SenderId, int UnreadCount)>> GetUnreadMessagesBySenderAsync(string receiverId)
        {
            var result = new List<(string SenderId, int UnreadCount)>();

            string query = @"
            SELECT senderId, COUNT(*) AS UnreadCount
            FROM Messages
            WHERE receiverId = @ReceiverId AND isRead = 0
            GROUP BY senderId;";

            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string senderId = reader.GetString("senderId");
                            int unreadCount = reader.GetInt32("UnreadCount");

                            result.Add((senderId, unreadCount));
                        }
                    }
                }
            }

            return result;
        }

        public static async Task<List<Message>> GetUnreadMessagesAsync(string senderId, string receiverId)
        {
            var messages = new List<Message>();

            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                await connection.OpenAsync();

                string query = @"
                SELECT senderId, receiverId, content, timestamp 
                FROM Messages 
                WHERE receiverId = @ReceiverId AND senderId = @SenderId AND isRead = 0;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);
                    command.Parameters.AddWithValue("@SenderId", senderId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var message = new Message(
                                reader.GetString("senderId"),
                                reader.GetString("receiverId"),
                                reader.GetString("content"),
                                reader.GetDateTime("timestamp")
                            );

                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }

        public static async Task MarkMessagesAsReadAsync(string receiverId, string senderId)
        {
            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                await connection.OpenAsync();

                string updateQuery = @"
                UPDATE Messages 
                SET isRead = 1 
                WHERE receiverId = @ReceiverId AND senderId = @SenderId AND isRead = 0;";

                using (var command = new MySqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);
                    command.Parameters.AddWithValue("@SenderId", senderId);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
