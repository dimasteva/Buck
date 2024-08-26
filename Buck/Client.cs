﻿using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MySqlConnector; // MySqlConnector namespace

namespace Buck
{
    public class Client
    {
        private string _username;
        private string _password;
        private string _name;
        private string _lastName;
        private string _email;

        public Client(string username, string password, string name, string lastName, string email)
        {
            _username = username;
            _password = password;
            _name = name;
            _lastName = lastName;
            _email = email;
        }

        public async Task<bool> CreateAccountAsync()
        {
            bool validUsernameAndMailResult = await IsUsernameOrEmailExistsAsync(_username, _email);
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
                        cmd.Parameters.AddWithValue("@Username", _username);
                        cmd.Parameters.AddWithValue("@Password", _password);
                        cmd.Parameters.AddWithValue("@Name", _name);
                        cmd.Parameters.AddWithValue("@LastName", _lastName);
                        cmd.Parameters.AddWithValue("@Email", _email);

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
    }
}
