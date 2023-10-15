using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;



namespace TheAddressBook
{
    public class AddressBook
    {
        private List<Contact> Contacts { get; set; } = new List<Contact>();

        private readonly IConfiguration _configuration;

        public AddressBook(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        
        public void AddContact(Contact contact)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Contacts (FirstName, LastName, Birthday) VALUES (@FirstName, @LastName, @Birthday)";
                if (connectionString == null)
                {
                    Console.WriteLine("Connection string not found. Please check your configuration.");
                }
                else {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                        command.Parameters.AddWithValue("@LastName", contact.LastName);
                        command.Parameters.AddWithValue("@Birthday", contact.Birthday);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        public void DisplayContacts()
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT FirstName, LastName, Birthday FROM Contacts";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]}, Birthday: {Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd")}");
                        }
                    }
                }
            }
        }

        public void SearchContacts(string query)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT FirstName, LastName, Birthday FROM Contacts WHERE FirstName LIKE @Query OR LastName LIKE @Query";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Adding a '%' wildcard to the query to match any characters before or after the query text
                    command.Parameters.AddWithValue("@Query", "%" + query + "%");
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No contacts found.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]}, Birthday: {Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd")}");
                        }
                    }
                }
            }
        }


        public void SearchContactsByMonth(int month)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlQuery = "SELECT FirstName, LastName, Birthday FROM Contacts WHERE MONTH(Birthday) = @Month";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("No contacts found.");
                            return;
                        }

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]}, Birthday: {Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd")}");
                        }
                    }
                }
            }

        }


        public void UpdateContact(string firstName, string lastName, Contact updatedContact)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Contacts SET FirstName = @NewFirstName, LastName = @NewLastName, Birthday = @NewBirthday WHERE FirstName = @FirstName AND LastName = @LastName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@NewFirstName", updatedContact.FirstName);
                    command.Parameters.AddWithValue("@NewLastName", updatedContact.LastName);
                    command.Parameters.AddWithValue("@NewBirthday", updatedContact.Birthday);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No contact found to update.");
                    }
                    else
                    {
                        Console.WriteLine("Contact updated successfully.");
                    }
                }
            }
        }


        public void DeleteContact(string firstName, string lastName)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Contacts WHERE FirstName = @FirstName AND LastName = @LastName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Contact deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No contact found with the given name.");
                    }
                }
            }
        }


        public List<Contact> GetBirthdaysThisMonth()
        {
            List<Contact> contacts = new List<Contact>();
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT FirstName, LastName, Birthday FROM Contacts WHERE MONTH(Birthday) = MONTH(GETDATE())";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Contact contact = new Contact
                            {
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Birthday = Convert.ToDateTime(reader["Birthday"])
                            };
                            contacts.Add(contact);
                        }
                    }
                }
            }
            return contacts;
        }




        //public void TestDatabaseConnection()
        //{
        //    string connectionString = _configuration.GetConnectionString("AddressBookDB");
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            Console.WriteLine("Database connection successful.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Database connection failed: " + ex.Message);
        //    }
        //}


    }
}
