# AddressBook

This was mainly made at first to practice c#, but to also store friends/family birthdays and maybe addresses in a local db. Feel free to use this code as you like
and if you want to connect it to your own DB use the instructions below - 




You will need to connect to your own local DB if you want to store anything. You can
do this in the in a few locations. 



You will NEED to update the Connection string in the appsettings.json file with your 
server and DB name


{
    "ConnectionStrings": {
        "AddressBookDB": "Server={server name};Database={database name};Integrated Security=true;TrustServerCertificate=true"
    }
}



I have commented out a test db method in the AddressBook.cs file
where you will need to update the instance of your db in the connection string line if you wanted to use it - 


string connectionString = _configuration.GetConnectionString("{DB Name here}");




