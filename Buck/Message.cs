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
        public string Receiver
        {
            get { return _receiver; }
            set { _receiver = value; }
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
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.WriteLine("PUKO SendMessageAsync" + e.Message);
                return false;
            }
            return true;
        }

        public static List<(string SenderId, int UnreadCount)> GetUnreadMessagesBySender(string receiverId)
        {
            var result = new List<(string SenderId, int UnreadCount)>();

            string query = @"
            SELECT senderId, COUNT(*) AS UnreadCount
            FROM Messages
            WHERE receiverId = @ReceiverId AND isRead = 0
            GROUP BY senderId;";

            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                connection.Open();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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

        public static List<Message> GetUnreadMessages(string senderId, string receiverId)
        {
            var messages = new List<Message>();
            try
            {
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    connection.Open();
                    //System.Diagnostics.Debug.WriteLine("otvorio konekciju");

                    string query = @"
                SELECT senderId, receiverId, content, timestamp 
                FROM Messages 
                WHERE receiverId = @ReceiverId AND senderId = @SenderId AND isRead = 0;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReceiverId", receiverId);
                        command.Parameters.AddWithValue("@SenderId", senderId);
                        System.Diagnostics.Debug.WriteLine("ovde je linija1");
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine("ovde je linija2");
                                var senderIdValue = reader.GetString("senderId");
                                System.Diagnostics.Debug.WriteLine(senderIdValue);
                                var receiverIdValue = reader.GetString("receiverId");
                                System.Diagnostics.Debug.WriteLine(receiverIdValue);
                                var contentValue = reader.GetString("content");
                                System.Diagnostics.Debug.WriteLine(contentValue);
                                DateTime timestampValue = DateTime.Now;
                                try
                                {
                                   timestampValue  = reader.GetDateTime("timestamp");
                                }
                                catch (Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine(e.ToString());
                                }
                                var message = new Message(senderIdValue, receiverIdValue, contentValue, timestampValue);
                                System.Diagnostics.Debug.WriteLine("ovde je linija3");

                                messages.Add(message);
                            }
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.WriteLine("PUKO GetUnreadMessages" + e.Message);
            }

            System.Diagnostics.Debug.WriteLine("uspesno se izvrsio getunreadmessages");
            return messages;
        }

        public static List<Message> GetAllReadMessages(string senderId, string receiverId, int numberOfMessages)
        {
            var messages = new List<Message>();
            try
            {
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    connection.Open();
                    // System.Diagnostics.Debug.WriteLine("otvorio konekciju");

                    // SQL query sa ORDER BY i LIMIT klauzulama
                    string query = @"
                SELECT senderId, receiverId, content, timestamp 
                FROM Messages 
                WHERE ((receiverId = @ReceiverId 
                AND senderId = @SenderId)
                OR (receiverId = @SenderId AND senderId = @ReceiverId))
                AND isRead = 1 
                ORDER BY timestamp DESC
                LIMIT @NumberOfMessages;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReceiverId", receiverId);
                        command.Parameters.AddWithValue("@SenderId", senderId);
                        command.Parameters.AddWithValue("@NumberOfMessages", numberOfMessages); // Dodajemo broj poruka kao parametar
                        System.Diagnostics.Debug.WriteLine("ovde je linija1");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //System.Diagnostics.Debug.WriteLine("ovde je linija2");
                                var senderIdValue = reader.GetString("senderId");
                                //System.Diagnostics.Debug.WriteLine(senderIdValue);
                                var receiverIdValue = reader.GetString("receiverId");
                                //System.Diagnostics.Debug.WriteLine(receiverIdValue);
                                var contentValue = reader.GetString("content");
                                //System.Diagnostics.Debug.WriteLine(contentValue);

                                DateTime timestampValue = DateTime.Now;
                                try
                                {
                                    timestampValue = reader.GetDateTime("timestamp");
                                }
                                catch (Exception e)
                                {
                                    System.Diagnostics.Debug.WriteLine(e.ToString());
                                }

                                var message = new Message(senderIdValue, receiverIdValue, contentValue, timestampValue);
                                System.Diagnostics.Debug.WriteLine("ovde je linija3");

                                messages.Add(message);
                            }
                        }
                    }
                }
            }
            catch (MySqlException e)
            {
                System.Diagnostics.Debug.WriteLine("PUKO GetUnreadMessages: " + e.Message);
            }
            messages.Reverse();
            return messages;
        }


        public static async Task MarkMessagesAsReadAsync(string senderId, string receiverId)
        {
            try
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
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("PUKO MarkMessagesAsReadAsync" + ex.ToString());
            }
        }
    }
}
