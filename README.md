# Queries <img src='https://candoumbe.visualstudio.com/_apis/public/build/definitions/c46372c6-db92-4b5d-88d5-3ec50e332749/7/badge'/>


This is a "basic" database agnostic query builder.

## Why ?
The idea of this project came to me when I dealt with Entity Framework 6.x Code First for a project I was working on as Architect.

We used Migrations to make changes to our database and sometimes we needed to write plain SQL statements as part of migrations.
For this, the EF 6.x <code>Sql(...)</code> command allows to add additional SQL statements that will be executed alongside the migrations. 
But I wasn't happy with that approach as the written SQL was tightly coupled to the database engine those migrations were run against. 
I wanted something more dynamic allowing to code SQL once in the migrations and be sure that it will run smoothly if we switch from SQL Server to Postgres.

### No more tightly coupled SQL string
Writing tightly coupled SQL means that you're writing SQL statements that are specific to a database engine. <br />

The following SQL string
```csharp
    string sql = "SELECT [Firstname] + ' ' + [Lastname] AS [fullname] FROM [members]"
```
is tightly coupled to SQL Server engine and won't work if dealing with Postgres whereas
```csharp
    // import the library
    import static Queries.Core.Builders.Fluent.QueryBuilder // C# 7 syntax
    import _ = Queries.Core.Builders.Fluent.QueryBuilder    // Pre C# 6 syntax

    //compute a "static" query
    IQuery query = Select(Concat("firstname").Field(), " ".Literal(), "lastname".Field()).From("members");
```
 will output
 
```csharp
    //For SQL SERVER
    string sqlServerString = query.ForSqlServer();
    Console.Writeline(sqlServerString); // SELECT [Firstname] + ' ' + [Lastname] AS [fullname] FROM [members]

    //For Postgres
    string postgresSqlString = query.ForPostgres(); 
    Console.Writeline(postgresSqlString);// SELECT "Firstname" + ' ' + "Lastname" "fullname" FROM "members"
```
### SQL queries secured by default

Most developers know about [SQL injection](https://en.wikipedia.org/wiki/SQL_injection) and how to protect from it.
But when using SQL string throughout one's codebase, it can quickly become a tedious task to secure each and every SQL query.<br />
Sure there's the ADO.NET library which provide various classes to create parameterized queries but this adds more and
more boilerplate code :

```csharp
using (var conn = GetConnectionSomehow() )
{
    conn.Open();

    DbParameter nicknameParam = new SqlParameter();
    nicknameParam.SqlDbType = SqlDbType.String;
    nicknameParam.ParameterName = "@nickname";
    nicknameParam.Value = "Bat%";

    SqlCommand cmd = new SqlCommand();
    cmd.Connection = conn;
    
    cmd.CommandText = "SELECT Firstname + ' ' + 'Lastname' FROM SuperHero WHERE Nickname LIKE @nickname";

    var result = cmd.ExecuteQuery();

    ....

    conn.Close();
}
```

whereas with Queries :

```csharp
using (var conn = GetConnectionSomehow() )
{
    conn.Open();

    IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field())
        .From("SuperHero")
        .Where("Nickname", Like, "Bat%" );
    cmd.CommandText = query.ForSqlServer();

    var result = cmd.ExecuteQuery();

    ....

    conn.Close();
}
```

The code is shorter, clearer as the boilerplate code is no longer a distraction

## Build queries

The `Queries.Core.Builders` namespace contains various classes that can be used to build queries. <br />
You can build various queries

- [SELECT](https://www.w3schools.com/sql/sql_select.asp) 
```csharp
// Using builders ...
IQuery query = new SelectQuery 
{
    Columns = new IColumn[]
    {
        new FieldColumn("Firstname"),
        new FieldColumn("Lastname")
    },
    Tables = new ITable[] 
    {
        new Table("members")
    }
};

// ... or fluent syntax
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members");
```
- UPDATE :
```csharp
// Using builders ...
IQuery query = new UpdateQuery 
{
    Table = new Table("members"),
    Values = new []
    {
        new UpdateFieldValue("Nickname".Field(), "Nightwing")
    },
    WhereCriteria = new WhereClause{ Column = "NickName", Operator = EqualTo, Value = "Robin" }
}

// ... or with fluent syntax
IQuery query = Update("members")
    .Set("Nickname".Field().EqualTo("NightWing"))
    .Where("Nickname", EqualTo, "Robin");
```

or even combine them using a [BatchQuery](./src/Queries.Core/Builders/BatchQuery.cs)

```csharp
BatchQuery batch = new BatchQuery(
    InsertInto("members").Values("Firstname".Field().EqualTo("Harley"), "Lastname".Field().EqualTo("Quinzel"))
    Delete("members_bkp").Where("Nickname".Field(), EqualTo, ""))
);
```
## Renderers
Renderers are special classes that can produce a SQL string given a [IQuery](./src/Queries.Core/IQuery.cs) instance. <br />

```csharp
IQuery query = GetQuery();
string sql = query.ForXXX() // where XXX stand for a database engine to target
```
### Settings
The "shape" of the SQL string (date format, parameterized query) can 
be customized by providing a [QueryRendererSettings](./src/Queries.Core/Renderers/QueryRendererSettings.cs) instance.
to the <code>ForXXXX()</code> method.
```csharp
IQuery query = ...
string sql = query.ForXXX(new QueryRendererSettings { PrettyPrint = false}) // where XXX stand for a database engine to target
```
### [SQL Server Query Renderer](https://www.nuget.org/packages/Queries.Renderers.SqlServer)
Builds SQL string that can be used with SQL Server Database Engine

```csharp
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

string sql = query.ForSqlServer(new QueryRendererSettings { PrettyPrint = true });

Console.WriteLine(sql); "DECLARE @p0 NUMERIC = 18; SELECT [Firstname] + ' ' + [Lastname] FROM members WHERE [Age] >= @p0" 
```
## How to install ?

1.  Download the [Queries.Core](https://www.nuget.org/packages/Queries.Core/) package and references it in your project.<br />
    From this point you can start building queries in your code.
2.  Download the Queries.Renderers.XXXXX that is specific to the database engine you're targeting.
    This will add extensions methods ForXXXX to all <code>IQuery</code> instances that produces SQL statements
3.  Enjoy !!!

# Contribute
Check out the [contribution guidelines](./CONTRIBUTING.md)
if you want to contribute to this project.

