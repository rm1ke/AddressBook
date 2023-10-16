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
        private readonly IConfiguration _configuration;

        public AddressBook(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddContact(Contact contact)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Contacts (FirstName, LastName, Birthday, Email, Address1, Address2, City, State, Zip) VALUES (@FirstName, @LastName, @Birthday, @Email, @Address1, @Address2, @City, @State, @Zip)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", contact.FirstName);
                    command.Parameters.AddWithValue("@LastName", contact.LastName);
                    command.Parameters.AddWithValue("@Birthday", contact.Birthday);
                    command.Parameters.AddWithValue("@Email", contact.Email);
                    command.Parameters.AddWithValue("@Address1", contact.Address1);
                    command.Parameters.AddWithValue("@Address2", contact.Address2);
                    command.Parameters.AddWithValue("@City", contact.City);
                    command.Parameters.AddWithValue("@State", contact.State);
                    command.Parameters.AddWithValue("@Zip", contact.Zip);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DisplayContacts()
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Contacts order by FirstName Asc";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var birthday = reader["Birthday"] == DBNull.Value ? "Unknown" : Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd");
                            var output = $"{reader["FirstName"]} {reader["LastName"]}, Birthday: {birthday}";
                            if (!string.IsNullOrEmpty(reader["Email"].ToString()))
                            {
                                output += $", Email: {reader["Email"]}";
                            }
                            if (!string.IsNullOrEmpty(reader["Address1"].ToString()))
                            {
                                output += $", Address: {reader["Address1"]}";
                            }
                            if (!string.IsNullOrEmpty(reader["Address2"].ToString()))
                            {
                                output += $" {reader["Address2"]}";
                            }
                            if (!string.IsNullOrEmpty(reader["City"].ToString()))
                            {
                                output += $", City: {reader["City"]}";
                            }
                            if (!string.IsNullOrEmpty(reader["State"].ToString()))
                            {
                                output += $", State: {reader["State"]}";
                            }
                            if (!string.IsNullOrEmpty(reader["Zip"].ToString()))
                            {
                                output += $", Zip: {reader["Zip"]}";
                            }
                            Console.WriteLine(output);
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
                string sqlQuery = "SELECT * FROM Contacts WHERE FirstName LIKE @Query OR LastName LIKE @Query";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
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
                            Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]}, Birthday: {Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd")}, Email: {reader["Email"]}, Address: {reader["Address1"]} {reader["Address2"]}, City: {reader["City"]}, State: {reader["State"]}, Zip: {reader["Zip"]}");
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
                string sqlQuery = "SELECT * FROM Contacts WHERE MONTH(Birthday) = @Month";
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
                            Console.WriteLine($"{reader["FirstName"]} {reader["LastName"]}, Birthday: {Convert.ToDateTime(reader["Birthday"]).ToString("yyyy-MM-dd")}, Email: {reader["Email"]}, Address: {reader["Address1"]} {reader["Address2"]}, City: {reader["City"]}, State: {reader["State"]}, Zip: {reader["Zip"]}");
                        }
                    }
                }
            }
        }

        public Contact GetContact(string firstName, string lastName)
        {
            string connectionString = _configuration.GetConnectionString("AddressBookDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Contacts WHERE FirstName = @FirstName AND LastName = @LastName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Contact
                            {
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                Birthday = reader["Birthday"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["Birthday"]),
                                Email = reader["Email"].ToString(),
                                Address1 = reader["Address1"].ToString(),
                                Address2 = reader["Address2"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Zip = reader["Zip"].ToString()
                            };
                        }
                        else
                        {
                            return null;  // or handle the case where the contact is not found
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
                string query = "UPDATE Contacts SET FirstName = @NewFirstName, LastName = @NewLastName, Birthday = @NewBirthday, Email = @NewEmail, Address1 = @NewAddress1, Address2 = @NewAddress2, City = @NewCity, State = @NewState, Zip = @NewZip WHERE FirstName = @FirstName AND LastName = @LastName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@NewFirstName", updatedContact.FirstName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewLastName", updatedContact.LastName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewBirthday", (updatedContact.Birthday.HasValue && updatedContact.Birthday.Value > DateTime.MinValue) ? updatedContact.Birthday.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewEmail", updatedContact.Email ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewAddress1", updatedContact.Address1 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewAddress2", updatedContact.Address2 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewCity", updatedContact.City ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewState", updatedContact.State ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@NewZip", updatedContact.Zip ?? (object)DBNull.Value);

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
                string query = "SELECT * FROM Contacts WHERE MONTH(Birthday) = MONTH(GETDATE())";
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
                                Birthday = Convert.ToDateTime(reader["Birthday"]),
                                Email = reader["Email"].ToString(),
                                Address1 = reader["Address1"].ToString(),
                                Address2 = reader["Address2"].ToString(),
                                City = reader["City"].ToString(),
                                State = reader["State"].ToString(),
                                Zip = reader["Zip"].ToString()
                            };
                            contacts.Add(contact);
                        }
                    }
                }
            }
            return contacts;
        }
    }
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


