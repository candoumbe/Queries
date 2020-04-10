# Queries [![Build Status](https://dev.azure.com/candoumbe/Queries/_apis/build/status/Queries?branchName=master)](https://dev.azure.com/candoumbe/Queries/_build/latest?definitionId=21&branchName=master)

This is a "basic" datastore agnostic query builder.

## Why?
The idea of this project came to me when I dealt with Entity Framework 6.x Code First for a project I was working on as Architect.

We used Migrations to make changes to our database and sometimes we needed to write plain SQL statements as part of migrations.
For this, the EF 6.x <code>Sql(...)</code> command allows to add additional SQL statements that will be executed alongside the migrations. 
But I wasn't happy with that approach as the written SQL was tightly coupled to the database engine those migrations were run against. 
I wanted something more dynamic allowing to code SQL once in the migrations and be sure that it will run smoothly if we switch from SQL Server to PostgreSQL / MySQL / ... .

### No more tightly coupled SQL string
Writing tightly coupled SQL means that you're writing SQL statements that are specific to a database engine. <br />

The following SQL string
```SQL
SELECT [Firstname] + ' ' + [Lastname] AS [fullname] FROM [members]
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
    
    /* CommandText now contains

     DECLARE @p0 NVARCHAR(max)
     SET @p0 = 'Bat%'
     SELECT Firstname + ' ' + 'Lastname' FROM SuperHero WHERE Nickname LIKE @p0

    */

    var result = cmd.ExecuteQuery();

    ....

    conn.Close();
}
```

The code is shorter, clearer as the boilerplate code is no longer a distraction

## Renderers
Renderers are special classes that can produce a SQL string given a [IQuery][class-iquery] instance. <br />

```csharp
IQuery query = GetQuery();
string sql = query.ForXXX() // where XXX stand for a database engine to target
```


### Settings
The "shape" of the output string (date format, parameterized query) can 
be customized by providing an implementation of [QueryRendererSettings][class-query-renderer-settings] instance.
to the <code>ForXXXX()</code> method.
```csharp
IQuery query = ...
QueryRendererSettings settings = new MyCustomRendererSettings();
string sql = query.ForXXX(settings) // where XXX stand for a database engine to target
```
### [SQL Server Query Renderer](https://www.nuget.org/packages/Queries.Renderers.SqlServer)
Builds SQL string that can be used with SQL Server Database Engine

```csharp
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

string sql = query.ForSqlServer(new QueryRendererSettings { PrettyPrint = true });

Console.WriteLine(sql); "DECLARE @p0 NUMERIC = 18; SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE [Age] >= @p0" 
```

### <a href='#' id='renderer-mysql'>MySQL Query Renderer</a>
Builds SQL string that can be used with MySQL Database Engine

```csharp
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

string sql = query.ForMySql(new QueryRendererSettings { PrettyPrint = true });

Console.WriteLine(sql); "DECLARE @p0 NUMERIC = 18; SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE [Age] >= @p0" 
```
## Build queries

### <a href='#' id='section-columns'>Columns</a>

[IColumn][class-columns-icolumn] is the base interface that all column like types implement.

[FieldColumn][class-columns-field]
 
Literals
Uses the following classes whenever you want to write a "raw" data in a query
- [BooleanColumn][class-columns-boolean] :  a column that can contains a boolean value.<br />
Use this class to output a boolean value in the query

```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("IsActive", EqualTo, new BooleanColumn(true));
```

can also be written 

```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("IsActive", EqualTo, true);
```
which will output for
```SQL
SELECT [Firstname], [Lastname] FROM [members] WHERE [IsActive] = 1
```
- [DateTimeColumn][class-columns-datetime] : a [IColumn][class-columns-icolumn] implementation that can contains a `date`/`time`/`datetime` value.<br />
Use this class to output a `DateTime`/`DateTimeOffset` value.

```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("DateOfBirth", EqualTo, 1.April(1990));
```

You can optionally specify a format to use when rendering the query with the `Format(string format)` extension method.
```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("DateOfBirth", EqualTo, 1.April(1990).Format("dd-MM-yyyy"));
```


The <code>Queries.Core.Builders</code> namespace contains various classes that can be used to build queries. <br />
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
- [UPDATE](https://www.w3schools.com/sql/sql_update.asp)
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

or even combine them using a [BatchQuery][class-builders-batch-query]

```csharp
BatchQuery batch = new BatchQuery(
    InsertInto("members").Values("Firstname".Field().EqualTo("Harley"), "Lastname".Field().EqualTo("Quinzel"))
    Delete("members_bkp").Where("Nickname".Field(), EqualTo, ""))
);
```
<strong>Warning</strong>
<code>IQuery</code> classes are all mutable (except when specified otherwise) meaning that any instance can be modified 
<strong>AFTER</strong> being created.
Use he <code>.Clone()</code> method to duplicate any instance.


### Criterias 
<code>Queries.Core.Parts.Clauses</code> namespace contains classes to add filters to <code>IQuery</code> instances.
#### Where
- [WhereClause][class-where-clause] : a criterion that will be applied to only one field of a [IQuery][class-iquery]
- [CompositeWhereClause][class-complex-where-clause] : combine several [WhereClause][class-where-clause] instances together.
#### Having
- [HavingClause][class-having-clause] : a criterion that will be applied to only one field of a [IQuery][class-iquery]
- [CompositeHavingClause][class-complex-having-clause] : combine several [HavingClause][class-having-clause] instances together.



## How to install ?

1.  Run <code>dotnet add package Queries.Core</code><br /> command to get the latest version of the [Queries.Core](https://www.nuget.org/packages/Queries.Core/) 
    package and references it in your project.<br />
    From this point you can start building queries in your code.
2.  Download the Queries.Renderers.XXXXX that is specific to the database engine you're targeting.
    This will add extensions methods ForXXXX to all [IQuery][class-iquery] instances that produces SQL statements
3.  Enjoy !!!

# Contribute
Check out the [contribution guidelines](./CONTRIBUTING.md)
if you want to contribute to this project.


[class-iquery]: ./src/Queries.Core/IQuery.cs
[class-where-clause]: ./src/Queries.Core/Parts/Clauses/WhereClause.cs
[class-having-clause]: ./src/Queries.Core/Parts/Clauses/HavingClause.cs
[class-complex-where-clause]: ./src/Queries.Core/Parts/Clauses/CompositeWhereClause.cs
[class-complex-having-clause]: ./src/Queries.Core/Parts/Clauses/CompositeHavingClause.cs
[class-query-renderer-settings]: ./src/Queries.Core/Renderers/QueryRendererSettings.cs
[class-builders-batch-query]: ./src/Queries.Core/Builders/BatchQuery.cs
[class-columns-icolumn]: ./src/Queries.Core/Parts/Columns/IColumn.cs
[class-columns-datetime]: ./src/Queries.Core/Parts/Columns/DateTimeColumn.cs
[class-columns-boolean]: ./src/Queries.Core/Parts/Columns/BooleanColumn.cs
[class-columns-field]: ./src/Queries.Core/Parts/Columns/FieldColumn.cs