# UST Software Technical Test
## Technology Used
An ASP.NET service has been created to implement a RESTful CDR API according to the brief. The language used was C# with .NET 6. Microsoft SQL Server was used to store call data, and Entity Framework (EF) Core with LINQ was used to define the table structure, make queries and parse the uploaded file into Call objects to be stored. It was created in Visual Studio Community 2022.

These technologies were chosen primarily to closely fit the technology stack that Union Street is using, and demonstrate my knowledge and skills in this area. However, they are very useful for meeting the brief specification and this stack is my first choice for implementing an API in a Windows environment.

In particular, EF Core with LINQ was vital in improving my productivity by avoiding the need to write low-level database access code. It is also very effective in preventing SQL injection attacks and related vulnerabilities.

## Assumptions
In cases where the caller_id was empty in the CSV file, it has been entered as “Unknown” to the database. On one line where the duration field was set to an invalid string, the duration has been entered as 0.

In a real scenario, I would first ask for clarification on what to do with lines containing the invalid data, whether that would be rejecting only the offending lines or rejecting the entire CSV file.

The program assumes that it will have access to a running SQLEXPRESS service to store its data. Other databases may be used by changing the connection string in appsettings.json.
## Running the Application
Visual Studio Community should be used to open the .sln file.

Under View -> Server Explorer, click Connect to Database and add a Microsoft SQL Server data source, and enter the server name. You can create a new database name e.g. “CDR” that the Calls table will be created under.

Install the Nuget Packages:
System.Data.SqlClient
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools

Create the Table:
(In Nuget Package Manager Console)
Add-Migration createdatabase -o Data/Migrations
Update-Database

The application should now run. It will open a web server and a browser frontend interface created with Swagger that will allow you to communicate with the endpoints.

### These endpoints are:
#### /api/Call (GET):
Returns the schema of a Call record.

#### /api/Call/UploadFile (POST):
Takes a CSV file as an input and parses the contents into the Calls table.

#### /api/Call/GetByReference (GET): 
Takes a reference input and returns the associated call record if a match is found.

#### /api/Call/GetCountDuration (GET):
Takes 2 input dates and an optional type. Finds records within the time period of the dates if they are within 31 days of each other. Additionally filters by type if it is included.
Returns the total duration and count of records found.

#### /api/Call/GetCallsByCallerID (GET):
Takes 2 input dates, a caller_id and an optional type. Finds and returns records matching the caller_id within the time period of the dates if they are within 31 days of each other. Additionally filters by type if it is included.

#### /api/Call/GetMostExpensiveCallsByCallerID (GET):
Takes 2 input dates, a caller_id, an integer n rows and an optional type. Finds and returns records matching the caller_id within the time period of the dates if they are within 31 days of each other. Does not return more records than the input n rows. Additionally filters by type if it is included.

## Future Considerations
With more time I would have tried connecting to an Azure SQL database rather than a local one, as well as the putting the ASP.NET application as a whole on Azure.

## Important Files
Controllers/CallController.cs:
In the MVC pattern, this is the controller which defines the API endpoints and their inputs. It takes input from users, queries the Calls table in the database and returns a result.

#### Models/Call.cs:
In the MVC pattern, this is the model which defines the data structure for each call. Each column is given a name and type. With EF Core this is used to create migrations and update the database structure any time that changes are made to the model.
This model also contains a function ParseFromCSV to parse each line of the uploaded file into a Call object.

#### Data/CallDbContext.cs:
This defines the database context that will be used to query the Calls table.

#### Appsettings.json:
Defines the SQL server connection string. It is configured to use Microsoft SQL Server Express and assumes this is running. Other databases may be connected to by changing the connection string; however only SQL Express has been tested.

#### Properties/launchSettings.json and Program.cs:
These are used to add the necessary services to the server instance, including Swagger and the SQL server connection.


