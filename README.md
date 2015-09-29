#Queries

This is a "basic" database agnostic query builder.

##Why ?
The idea of this project came to me when I dealt with Entity Framework 6.x Code First for a project I was working on as Architect.

We used Migrations to make changes to our database and sometimes we needed to write plain SQL statements as part of exist.
For this, the EF 6.x <code>Sql(...)</code> command allows to add additional SQL statements that will be executed alongside the migrations. But I wasn't happy with that approach as the written SQL was tightly coupled to the database's engine migrations were run against. 
I wanted something more dynamic allowing to code SQL once in the migrations and be sure that it will run if we switch from postgres

###Tightly coupled vs loosely coupled SQL
Writing tightly coupled SQL means that you're writing SQL statements that are specific to a database engine.
Example : 

The following SQL string

```csharp
    string sql = "SELECT [Firstname] + ' ' + [Lastname] AS [fullname] FROM [members]"
```
is tightly coupled to SQL Server engine and won't work if dealing with Postgres whereas
```csharp
    //we compute a "static" query
    IQuery query = Select(Concat("firstname").Field(), " ".Literal(), "lastname".Field()).From("members");
```
 will output
 
```csharp
    //For SQL SERVER
    string sqlServerString = query.ForSqlServer();
    Console.Writeline(sqlServerString); // will output : SELECT [Firstname] + ' ' + [Lastname] AS [fullname] FROM [members]
```
```csharp
    //For Postgres
    string postgresSqlString = query.ForPostgres(); 
    Console.Writeline(postgresSqlString);// will output "SELECT "Firstname" + ' ' + "Lastname" "fullname" FROM "members"
```

##How to install ?

1.  Download the Queries.Core package
    From this point you can already build queries
2.  Download the Queries.Renderers.XXXXX that is specific to the database you're targeting.
    This will add extensions methods ForXXXX to all <code>IQuery</code> isntances that produces SQL statements
3.  Enjoy !!!
    
