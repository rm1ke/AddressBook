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
                Console.WriteLine("7. Exit");
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
                        Contact newContact = new Contact
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            Birthday = birthday
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
                        Console.Write("Enter new first name: ");
                        string updateFirstName = Console.ReadLine();
                        Console.Write("Enter new last name: ");
                        string updateLastName = Console.ReadLine();
                        Console.Write("Enter new birthday (yyyy-mm-dd): ");
                        DateTime updateBirthday;
                        string birthdayInput = Console.ReadLine();
                        while (!DateTime.TryParse(birthdayInput, out updateBirthday))
                        {
                            Console.Write("Invalid date, please enter again (yyyy-mm-dd): ");
                            birthdayInput = Console.ReadLine();
                        }
                        Contact updatedContact = new Contact
                        {
                            FirstName = updateFirstName,
                            LastName = updateLastName,
                            Birthday = updateBirthday
                        };
                        addressBook.UpdateContact(oldFirstName, oldLastName, updatedContact);
                        break;
                    case "6":
                        Console.Write("Enter first name: ");
                        string deleteFirstName = Console.ReadLine();
                        Console.Write("Enter last name: ");
                        string deleteLastName = Console.ReadLine();
                        addressBook.DeleteContact(deleteFirstName, deleteLastName);
                        break;
                    case "7":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }


            // additional code for user interaction here
        }
    }
}