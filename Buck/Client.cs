using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector; // MySqlConnector namespace

namespace Buck
{
    public class Client
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }


        public Client(string username, string password, string name, string lastName, string email)
        {
            Username = username;
            Password = password;
            Name = name;
            LastName = lastName;
            Email = email;
        }

        public async Task<bool> CreateAccountAsync()
        {
            bool validUsernameAndMailResult = await IsUsernameOrEmailExistsAsync(Username, Email);
            if (!validUsernameAndMailResult)
            {
                return false;
            }
            using (var conn = new MySqlConnection(LoginPage.connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    string query = "INSERT INTO Client (username, password, clientName, clientLastName, email) " +
                    "VALUES (@Username, @Password, @Name, @LastName, @Email);";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Name", Name);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@Email", Email);

                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine("MySQL Error: " + ex.ToString());
                    return false;
                }
            }
        }

        private async Task<bool> IsUsernameOrEmailExistsAsync(string username, string email)
        {
            using (var conn = new MySqlConnection(LoginPage.connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM Client " +
                                   "WHERE username = @Username OR email = @Email";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Email", email);

                        long count = (long)await cmd.ExecuteScalarAsync();

                        return count == 0;
                    }
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MySQL Error: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        public static async Task<bool> DoesAccountExistAsync(string username, string password)
        {
            using (var conn = new MySqlConnection(LoginPage.connectionString))
            {
                try
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) " +
                                   "FROM Client " +
                                   "WHERE username = @Username AND password = @Password";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        long count = (long)await cmd.ExecuteScalarAsync();

                        return count > 0;
                    }
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"MySQL Error: {ex.Message}");
                    return false;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                    return false;
                }
            }
        }

        public static async Task<List<string>> SearchClientsAsync(string startsWith)
        {
            int usersToBeReturned = 10;
            var users = new List<string>();

            string query = "SELECT username FROM Client WHERE username LIKE @StartsWith LIMIT @Limit";
            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartsWith", startsWith + "%");
                    command.Parameters.AddWithValue("@Limit", usersToBeReturned);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(reader.GetString("username"));
                        }
                    }
                }

            }
            return users;
        }

        public static async Task<Dictionary<string, string>> GetDataWithUsername(string username)
        {
            var result = new Dictionary<string, string>();

            string query = "SELECT username, email, clientName, clientLastName, password FROM Client WHERE username = @Username LIMIT 1";

            using (var connection = new MySqlConnection(LoginPage.connectionString))
            {
                await connection.OpenAsync();

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            result["username"] = reader.GetString("username");
                            result["email"] = reader.GetString("email");
                            result["clientName"] = reader.GetString("clientName");
                            result["clientLastName"] = reader.GetString("clientLastName");
                            result["password"] = reader.GetString("password");
                        }
                    }
                }
            }

            return result.Count > 0 ? result : null; // Vraća null ako nema rezultata
        }

        public async Task<bool> SyncDataAsync()
        {
            string query = @"
            UPDATE Client 
            SET 
                password = @Password, 
                clientName = @Name, 
                clientLastName = @LastName, 
                email = @Email
            WHERE 
                username = @Username;";

            try
            {
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new MySqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@Username", Username);
                        command.Parameters.AddWithValue("@Password", Password);
                        command.Parameters.AddWithValue("@Name", Name);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@Email", Email);

                        // Execute the query asynchronously
                        int result = await command.ExecuteNonQueryAsync();

                        // Check if any rows were affected
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or display the error as needed
                System.Diagnostics.Debug.WriteLine("Error", $"Failed to sync data: {ex.Message}", "OK");
                return false;
            }
        }

        public static async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            // Check if the email and new password are valid
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            // Create SQL command to update the password for the specified email
            string query = "UPDATE Client SET password = @newPassword WHERE email = @Email";

            try
            {
                // Create a connection to the database
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    await connection.OpenAsync();

                    // Create a MySQL command with parameters
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newPassword", newPassword);
                        command.Parameters.AddWithValue("@Email", email);

                        // Execute the command and check the number of affected rows
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Return true if at least one row was updated, otherwise false
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception e)
            {
                //System.Diagnostics.Debug.WriteLine("ALOOOOO" + e.ToString());
                return false;
            }
        }

        public static async Task<bool> IsEmailPresentAsync(string email)
        {
            // Proveri da li je email validan
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // SQL upit za proveru postojanja emaila u tabeli
            string query = "SELECT COUNT(*) FROM Client WHERE email = @Email";

            try
            {
                // Kreiraj konekciju ka bazi
                using (var connection = new MySqlConnection(LoginPage.connectionString))
                {
                    await connection.OpenAsync();

                    // Kreiraj MySQL komandu sa parametrima
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        // Izvrši komandu i dobavi broj redova
                        var result = await command.ExecuteScalarAsync();

                        // Vraća true ako je email prisutan (tj. ako COUNT > 0)
                        return Convert.ToInt32(result) > 0;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ALOOOOO" + e.ToString());
                return false;
            }
        }
    }
}
