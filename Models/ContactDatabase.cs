using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ContactBookApp.Models
{
    public class ContactDatabase
    {
        private MySqlConnection connection;

        public ContactDatabase()
        {
            string connectionString = "SERVER=localhost;DATABASE=ContactBookDB;UID=root;PASSWORD=password;";
            connection = new MySqlConnection(connectionString);
        }

        public void AddContact(Contact contact)
        {
            string query = $"INSERT INTO Contacts (FirstName, LastName, PhoneNumber, EmailAddress, BirthDate) VALUES ('{contact.FirstName}', '{contact.LastName}', '{contact.PhoneNumber}', '{contact.EmailAddress}', '{contact.BirthDate.ToString("yyyy-MM-dd")}')";
            ExecuteNonQuery(query);
        }

        public void UpdateContact(Contact contact)
        {
            string query = $"UPDATE Contacts SET FirstName='{contact.FirstName}', LastName='{contact.LastName}', PhoneNumber='{contact.PhoneNumber}', EmailAddress='{contact.EmailAddress}', BirthDate='{contact.BirthDate.ToString("yyyy-MM-dd")}' WHERE Id={contact.Id}";
            ExecuteNonQuery(query);
        }

        public void DeleteContact(int id)
        {
            string query = $"DELETE FROM Contacts WHERE Id={id}";
            ExecuteNonQuery(query);
        }

        public List<Contact> GetContacts()
        {
            string query = "SELECT * FROM Contacts";
            List<Contact> contacts = new List<Contact>();

            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                contacts.Add(new Contact
                {
                    Id = dataReader.GetInt32("Id"),
                    FirstName = dataReader.GetString("FirstName"),
                    LastName = dataReader.GetString("LastName"),
                    PhoneNumber = dataReader.GetString("PhoneNumber"),
                    EmailAddress = dataReader.GetString("EmailAddress"),
                    BirthDate = dataReader.GetDateTime("BirthDate")
                });
            }

            dataReader.Close();
            connection.Close();

            return contacts;
        }

        private void ExecuteNonQuery(string query)
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}