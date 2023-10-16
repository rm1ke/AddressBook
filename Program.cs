using TheAddressBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtentions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Data.SqlClient;
using System.IO;






namespace TheAddressBook
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();


            AddressBook addressBook = new AddressBook(configuration);
            //Console.WriteLine(configuration.GetConnectionString("AddressBookDB"));
            //addressBook.TestDatabaseConnection();
            while (true) // this creates an infinite loop that will keep displaying the menu
            {
                Console.Clear(); // clears the console screen - keeps menu tidy
                Console.WriteLine("1. Add a new contact");
                Console.WriteLine("2. View all contacts");
                Console.WriteLine("3. Search for a contact");
                Console.WriteLine("4. Search by month");
                Console.WriteLine("5. Update a contact");
                Console.WriteLine("6. Delete a contact");
                Console.WriteLine("7. Search for Birthdays this month");
                Console.WriteLine("8. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Enter first name: ");
                        string firstName = Console.ReadLine();
                        Console.Write("Enter Last name: ");
                        string lastName = Console.ReadLine();
                        Console.Write("Enter birthday (yyyy-mm-dd): ");
                        DateTime birthday;
                        while (!DateTime.TryParse(Console.ReadLine(), out birthday))
                        {
                            Console.Write("Invalid date, please enter again (yyyy-mm-dd): ");
                        }
                        Console.Write("Enter email: ");
                        string email = Console.ReadLine();
                        Console.Write("Enter Address: ");
                        string address1 = Console.ReadLine();
                        Console.Write("Apt or Building?: ");
                        string address2 = Console.ReadLine();
                        Console.Write("Enter City: ");
                        string city = Console.ReadLine();
                        Console.Write("Enter State: ");
                        string state = Console.ReadLine();
                        Console.Write("Enter ZipCode: ");
                        string zip = Console.ReadLine();
                        Contact newContact = new Contact
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Birthday = birthday,
                            Email = email,
                            Address1 = address1,
                            Address2 = address2,
                            City = city,
                            State = state,
                            Zip = zip
                        };
                        addressBook.AddContact(newContact);
                        Console.WriteLine("Contact added successfully.");
                        break;
                    case "2":
                        addressBook.DisplayContacts();
                        break;
                    case "3":
                        Console.Write("Enter name to search: ");
                        string query = Console.ReadLine();
                        addressBook.SearchContacts(query);
                        break;
                    case "4":
                        Console.Write("Enter month (01-12): ");
                        string monthInput = Console.ReadLine();
                        int month;
                        while (!int.TryParse(monthInput, out month) || month < 1 || month > 12)
                        {
                            Console.Write("Invalid month, please enter again (01-12): ");
                            monthInput = Console.ReadLine();
                        }
                        addressBook.SearchContactsByMonth(month);
                        break;
                    case "5":
                        Console.Write("Enter the first name of the contact you want to update: ");
                        string oldFirstName = Console.ReadLine();
                        Console.Write("Enter the last name of the contact you want to update: ");
                        string oldLastName = Console.ReadLine();

                        Contact currentContact = addressBook.GetContact(oldFirstName, oldLastName);
                        if (currentContact == null)
                        {
                            Console.WriteLine("No contact found to update.");
                            break;
                        }

                        // Ask user if they want to update the first name
                        Console.Write("Update first name? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            Console.Write("Enter new first name: ");
                            currentContact.FirstName = Console.ReadLine();
                        }

                        // Ask user if they want to update the last name
                        Console.Write("Update last name? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            Console.Write("Enter new last name: ");
                            currentContact.LastName = Console.ReadLine();
                        }

                        // Ask user if they want to update the birthday
                        Console.Write("Update birthday? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            Console.Write("Enter new birthday (yyyy-mm-dd): ");
                            DateTime updateBirthday;
                            while (!DateTime.TryParse(Console.ReadLine(), out updateBirthday))
                            {
                                Console.Write("Invalid date, please enter again (yyyy-mm-dd): ");
                            }
                            currentContact.Birthday = updateBirthday;
                        }

                        // Ask user if they want to update the email
                        Console.Write("Update email? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            Console.Write("Enter new email: ");
                            currentContact.Email = Console.ReadLine();
                        }

                        // Ask user if they want to update the address
                        Console.Write("Update address? (yes/no): ");
                        if (Console.ReadLine().ToLower() == "yes")
                        {
                            Console.Write("Enter new Address 1: ");
                            currentContact.Address1 = Console.ReadLine();
                            Console.Write("Enter new Address 2: ");
                            currentContact.Address2 = Console.ReadLine();
                            Console.Write("Enter new City: ");
                            currentContact.City = Console.ReadLine();
                            Console.Write("Enter new State: ");
                            currentContact.State = Console.ReadLine();
                            Console.Write("Enter new Zip: ");
                            currentContact.Zip = Console.ReadLine();
                        }

                        addressBook.UpdateContact(oldFirstName, oldLastName, currentContact);
                        break;
                    case "6":
                        Console.Write("Enter first name: ");
                        string deleteFirstName = Console.ReadLine();
                        Console.Write("Enter last name: ");
                        string deleteLastName = Console.ReadLine();
                        addressBook.DeleteContact(deleteFirstName, deleteLastName);
                        break;
                    case "7":
                        List<Contact> birthdaysThisMonth = addressBook.GetBirthdaysThisMonth();
                        if (birthdaysThisMonth.Count == 0)
                        {
                            Console.WriteLine("No birthdays this month.");
                        }
                        else
                        {
                            Console.WriteLine("Birthdays this month:");
                            foreach (Contact contact in birthdaysThisMonth)
                            {
                                Console.WriteLine($"Name: {contact.FirstName} {contact.LastName}");
                                Console.WriteLine($"Birthday: {contact.Birthday?.ToString("yyyy-MM-dd")}");
                                if (!string.IsNullOrEmpty(contact.Email))
                                {
                                    Console.WriteLine($"Email: {contact.Email}");
                                }
                                if (!string.IsNullOrEmpty(contact.Address1) || !string.IsNullOrEmpty(contact.Address2))
                                {
                                    Console.WriteLine($"Address: {contact.Address1} {contact.Address2}");
                                }
                                 if (!string.IsNullOrEmpty(contact.City))
                                {
                                    Console.WriteLine($"City: {contact.City}");
                                }
                                if (!string.IsNullOrEmpty(contact.State))
                                {
                                    Console.WriteLine($"State: {contact.State}");
                                }
                                if (!string.IsNullOrEmpty(contact.Zip))
                                {
                                    Console.WriteLine($"Zip: {contact.Zip}");
                                }
                                Console.WriteLine(new string('-', 20));  // Output a separator for readability
                            }
                        }
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }


           
        }
    }
}