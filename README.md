# AddressBook

You will need to connect to your own local DB if you want to store anything. You can
do this in the in a few locations. 

I have commented out a test db method in the AddressBook.cs file
where you will need to update the instance of your db in the connection string line - 

string connectionString = _configuration.GetConnectionString("{DB Name here}");

You will need to update the Connection string in the appsettings.json file with your 
server and DB name


{
    "ConnectionStrings": {
        "AddressBookDB": "Server={server name};Database={database name};Integrated Security=true;TrustServerCertificate=true"
    }
}

