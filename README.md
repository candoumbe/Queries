# Queries <!-- omit in toc -->

![GitHub Main branch Status](https://img.shields.io/github/actions/workflow/status/candoumbe/queries/delivery.yml?branch=main&label=main)
![GitHub Develop branch Status](https://img.shields.io/github/actions/workflow/status/candoumbe/queries/integration.yml?branch=develop&label=develop)
[![codecov](https://codecov.io/gh/candoumbe/DataFilters/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/DataFilters)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/queries)](https://github.com/candoumbe/queries/issues)
[![DataFilters ](https://img.shields.io/nuget/vpre/queries.core?label=Queries.Core)](https://nuget.org/packages/queries.core)
This is a "basic" datastore agnostic query builder.

**Table of contents**
- [<a href="#" id="lnk-why">Why?</a>](#why)
- [<a href="#" id="lnk-coupling">No more tightly coupled SQL string</a>](#no-more-tightly-coupled-sql-string)
- [<a href="#" id="lnk-secured">SQL queries secured by default</a>](#sql-queries-secured-by-default)
- [Building statements](#building-statements)
  - [<a href="#" id='section-columns'>Columns</a>](#columns)
    - [<a href="#">Statements</a>](#statements)
      - [<a href="#" id='select-query'>Select</a>](#select)
      - [<a href="#" id='update-query'>Update</a>](#update)
      - [<a href="#" id="delete-query">Delete</a>](#delete)
      - [<a href="#" id="insert-query">Insert</a>](#insert)
      - [Combine](#combine)
    - [<a href="#" id='#criterias'>Criterias</a>](#criterias)
      - [<a href="#" id='where'>Where</a>](#where)
      - [<a href="#" id='where'>Having</a>](#having)
    - [<a href='#'>Functions</a>](#functions)
  - [<a href="#" id='rendering-statements'>Rendering statements</a>](#rendering-statements)
    - [<a href="#" id='renderer-sql-server'> SQL Server Query Renderer</a>](#-sql-server-query-renderer)
    - [<a href="#" id='renderer-mysql'>MySQL Query Renderer</a>](#mysql-query-renderer)
    - [<a href="#" id='renderer-mysql'>Sqlite Query Renderer</a>](#sqlite-query-renderer)
    - [QueryRendererSettings](#queryrenderersettings)
  - [How to install ?](#how-to-install-)
- [Contribute](#contribute)
- [What's new](#whats-new)

# <a href="#" id="lnk-why">Why?</a>

The idea of this project came to me when I dealt with Entity Framework 6.x Code First for a project I was working on as Architect.

We used Migrations to make changes to our database and sometimes we needed to write plain SQL statements as part of migratinuons.
For this, the EF 6.x <code>Sql(...)</code> command allows to add additional SQL statements that will be executed alongside the migrations. 
But I wasn't happy with that approach as the written SQL was tightly coupled to the database engine those migrations were run against. 
I wanted something more dynamic allowing to code SQL once in the migrations and be sure that it will run smoothly if we switch from SQL Server to PostgreSQL / MySQL / ... .

# <a href="#" id="lnk-coupling">No more tightly coupled SQL string</a>
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
# <a href="#" id="lnk-secured">SQL queries secured by default</a>

Most developers know about [SQL injection](https://en.wikipedia.org/wiki/SQL_injection) and how to protect from it.
But when using SQL string throughout one's codebase, it can quickly become a tedious task to secure each and every SQL query.<br />
Sure there's the ADO.NET library which provide various classes to create parameterized queries but this add more and more boilerplate code :

```csharp
using (var conn = GetConnectionSomehow() )
{
    conn.Open();

    DbParameter nicknameParam = new SqlParameter();
    nicknameParam.SqlDbType = SqlDbType.String;
    nicknameParam.ParameterName = "@nickname";
    nicknameParam.Value = "Bat%";

    SqlCommand cmd = new SqlCommand()
    cmd.Connection = conn;
    
    cmd.CommandText = "SELECT Firstname + ' ' + 'Lastname' FROM SuperHero WHERE Nickname LIKE @nickname";

    var result = cmd.ExecuteQuery();

    ....

    conn.Close();
}
```

whereas with `Queries` :

```csharp
using (var conn = GetConnectionSomehow() )
{
    conn.Open();

    IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field())
        .From("SuperHero")
        .Where("Nickname", Like, "Bat%" );

    cmd.CommandText = query.ForSqlServer();
    
    /* CommandText now contains

     DECLARE @p0 NVARCHAR(max);
     SET @p0 = 'Bat%';
     SELECT Firstname + ' ' + 'Lastname' FROM SuperHero WHERE Nickname LIKE @p0;

    */

    var result = cmd.ExecuteQuery();

    ....

    conn.Close();
}
```

The code is shorter, clearer as the boilerplate code is no longer a distraction

# Building statements

## <a href="#" id='section-columns'>Columns</a>

[IColumn][class-columns-icolumn] is the base interface that all column like types implement.

- <a href="#" id='columns-field'> [FieldColumn][class-columns-field]</a> contains a column name.

- <a href="#" id='columns-literal'>LiteralColumn</a>
Uses the following classes whenever you want to write a "raw" data in a query

- <a href="#" id='columns-boolean'>[BooleanColumn][class-columns-boolean]</a> : a column that can contains a boolean value.<br />
 
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

- [StringColumn][class-columns-string] : an [IColumn][class-columns-icolumn] implementation that contains "string" values

```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("DateOfBirth", EqualTo, 1.April(1990));
```

You can optionally specify a format to use when rendering the variable with the `Format(string format)` extension method.
```csharp
IQuery query = Select("Firstname".Field(), "Lastname".Field())
    .From("members")
    .Where("DateOfBirth", EqualTo, 1.April(1990).Format("dd-MM-yyyy"));
```

💡 Use the column type most suitable to your need to leverage both intellisence and the fluent builder API.

### <a href="#">Statements</a>

You can start building various statements after [installing](#how-to-install-) the Queries.Core package.

#### <a href="#" id='select-query'>[Select][class-select-query]</a>

Create a [`SelectQuery`][class-select-query] instance either by using the builder or the fluent syntax to build (drum rolling ...) a [SELECT query](https://www.w3schools.com/sql/sql_select.asp)

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
#### <a href="#" id='update-query'>[Update][class-update-query]</a>

Create a [`UpdateQuery`][class-update-query] instance either by using the builder or the fluent syntax to build (drum rolling ...) an [UPDATE](https://www.w3schools.com/sql/sql_update.asp) statement
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

#### <a href="#" id="delete-query">[Delete][class-delete-query]</a>

Create a [`DeleteQuery`][class-delete-query] instance either by using the builder or the fluent syntax to build (drum rolling ...) an [DELETE](https://www.w3schools.com/sql/sql_delete.asp) statement

```csharp
// Using builders ...
IQuery query = new DeleteQuery 
{
    Table = new Table("members"),
    WhereCriteria = new WhereClause{ Column = "Activity", Operator = NotLike, Value = "%Super hero%" }
}

// ... or with fluent syntax
IQuery query = Delete("members")
               .Where("Activity".Field(), NotLike, "%Super hero%")

```

#### <a href="#" id="insert-query">[Insert][class-insert-query]</a>

Create a [`InsertIntoQuery`][class-insert-query] instance either by using the builder or the fluent syntax to build (drum rolling ...) an [INSERT INTO](https://www.w3schools.com/sql/sql_insert.asp) statement

```csharp
// Using builders ...
IQuery query = new InsertIntoQuery

// ... or with fluent syntax
IQuery query = InsertInto("members")
                    .Values(
                        "Firstname".Field().InsertValue("Bruce".Literal()),
                        "Lastname".Field().InsertValue("Wayne".Literal())
                    )

```


or even combine them using a [BatchQuery][class-builders-batch-query]


#### [BatchQuery][class-batch-query] 

```csharp
BatchQuery batch = new BatchQuery(
    InsertInto("members").Values("Firstname".Field().EqualTo("Harley"), "Lastname".Field().EqualTo("Quinzel"))
    Delete("members_bkp").Where("Nickname".Field(), EqualTo, ""))
);
```

**Warning**

All `xxxxQuery` classes are all mutable (unless specified otherwise) meaning that any instance can be modified 
**AFTER** being created.
Use the <code>.Clone()</code> method to duplicate any instance.


### <a href="#" id='#criterias'>Criterias</a> 
<code>Queries.Core.Parts.Clauses</code> namespace contains classes to add filters to <code>IQuery</code> instances.
#### <a href="#" id='where'>Where</a>

- [WhereClause][class-where-clause] : a criterion that will be applied to only one field of a [IQuery][class-iquery]
- [CompositeWhereClause][class-complex-where-clause] : combine several [IWhereClause][class-iwhere-clause] instances together.

#### <a href="#" id='where'>Having</a>

- [HavingClause][class-having-clause] : a criterion that will be applied to only one field of a [IQuery][class-iquery]
- [CompositeHavingClause][class-complex-having-clause] : allow to combine several [IHavingClause][class-ihaving-clause] instances together.


### <a href='#'>Functions</a>

Several functions are supported out of the box. See [IFunction][class-functions] implementations and associated unit tests to see how to use them when building statemeents.


💡 You can always use [NativeQuery](src/Queries.Core/Builders/NativeQuery.cs) whenever you need to write a statement that is not yet supported by the libray.

## <a href="#" id='rendering-statements'>Rendering statements</a>
Renderers are special classes that can produce a SQL string given a [IQuery][class-iquery] instance.

```csharp
IQuery query = GetQuery();
string sql = query.ForXXX() // where XXX stand for a database engine to target
```

### <a href="#" id='renderer-sql-server'> [SQL Server Query Renderer](https://www.nuget.org/packages/Queries.Renderers.SqlServer)</a>
Builds SQL string that can be used with SQL Server Database Engine

```csharp
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

string sql = query.ForSqlServer(new QueryRendererSettings { PrettyPrint = true });

Console.WriteLine(sql);

/*

DECLARE @p0 NUMERIC = 18;
SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE [Age] >= @p0

*/ 
```

### <a href="#" id='renderer-mysql'>[MySQL Query Renderer](https://www.nuget.org/packages/Queries.Renderers.MySql)</a>
Builds SQL string that can be used with MySQL Database Engine

```csharp
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

string sql = query.ForMySql(new QueryRendererSettings { PrettyPrint = true });

Console.WriteLine(sql);

/*

DECLARE @p0 NUMERIC = 18;
SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE [Age] >= @p0" 

*/
```

### <a href="#" id='renderer-mysql'>[Sqlite Query Renderer](https://www.nuget.org/packages/Queries.Renderers.Sqlite)</a>
Builds SQL string that can be used with Sqlite Database Engine

```csharp
IQuery query = Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("Fullname"))
                    .From("superheroes")
                    .Where("nickname".Field(), EqualTo, "Batman");

string sql = query.ForSqlite(new SqliteRendererSettings { PrettyPrint = true });

Console.WriteLine(sql); 

/*

BEGIN; 
PRAGMA temp_store = 2; 
CREATE TEMP TABLE "_VARIABLES"(ParameterName TEXT PRIMARY KEY, RealValue REAL, IntegerValue INTEGER, BlobValue BLOB, TextValue TEXT); 
INSERT INTO "_VARIABLES" ("ParameterName") VALUES ('p0'); 
UPDATE "_VARIABLES" SET "TextValue" = 'Batman' WHERE ("ParameterName" = 'p0')
SELECT "firstname" || ' ' || "lastname" AS "Fullname" " 
FROM "superheroes" " 
WHERE ("nickname" = (SELECT COALESCE("RealValue", "IntegerValue", "BlobValue", "TextValue") FROM "_VARIABLES" WHERE ("ParameterName" = 'p0') LIMIT 1));
DROP TABLE "_VARIABLES";
END

*/
```


💡 There are several renderers already available on [nuget.org](https://www.nuget.org/packages?q=Queries.Renderers).


### QueryRendererSettings
The "shape" of the string returned by a [renderer](#rendering-statements) (date format, parameterized query, ...) can 
be customized by providing an implementation of [QueryRendererSettings][class-query-renderer-settings] instance.
to the <code>ForXXXX()</code> method.
```csharp
IQuery query = ...
QueryRendererSettings settings = new MyCustomRendererSettings();
string sql = query.ForXXX(settings) // where XXX stand for a database engine to target
```

- `DateFormatString` : defines how DateTimes should be printed (`YYYY-MM-DD` by default)
- `FieldnameCasingStrategy` : Defines the column name casing strategy (`Default` meaning no transformation)
- `PrettyPrint`
- `Parametrization` : a hint for renderers on how to handle all variables a [IQuery] my embbed. 
This is useful when variables declaration has already been taken care of (see [CollectVariableDeclaration](#collect-variables)) 


## How to install ?

1.  Run `dotnet add package Queries.Core`  command to get the latest version of the [Queries.Core](https://www.nuget.org/packages/Queries.Core/) 
    package and references it in your project.<br />
    From this point you can start building queries in your code.
2.  Download the Queries.Renderers.XXXXX that is specific to the database engine you're targeting.
    This will add extensions methods `ForXXXX` to all [IQuery][class-iquery] instances that produces SQL statements
3.  Enjoy !!!

# Contribute
Check out the [contribution guidelines](./CONTRIBUTING.md) if you want to contribute to this project.

# What's new
Check out the [changelog](CHANGELOG.md) to see what's new

[class-iquery]: ./src/Queries.Core/IQuery.cs
[class-batch-query]: ./src/Queries.Core/Builders/BatchQuery.cs
[class-select-query]: ./src/Queries.Core/Builders/SelectQuery.cs
[class-update-query]: ./src/Queries.Core/Builders/UpdateQuery.cs
[class-delete-query]: ./src/Queries.Core/Builders/DeleteQuery.cs
[class-insert-query]: ./src/Queries.Core/Builders/InsertIntoQuery.cs
[class-where-clause]: ./src/Queries.Core/Parts/Clauses/WhereClause.cs
[class-iwhere-clause]: ./src/Queries.Core/Parts/Clauses/IWhereClause.cs
[class-having-clause]: ./src/Queries.Core/Parts/Clauses/HavingClause.cs
[class-ihaving-clause]: ./src/Queries.Core/Parts/Clauses/IHavingClause.cs
[class-complex-where-clause]: ./src/Queries.Core/Parts/Clauses/CompositeWhereClause.cs
[class-complex-having-clause]: ./src/Queries.Core/Parts/Clauses/CompositeHavingClause.cs
[class-query-renderer-settings]: ./src/Queries.Core/Renderers/QueryRendererSettings.cs
[class-builders-batch-query]: ./src/Queries.Core/Builders/BatchQuery.cs
[class-columns-icolumn]: ./src/Queries.Core/Parts/Columns/IColumn.cs
[class-columns-datetime]: ./src/Queries.Core/Parts/Columns/DateTimeColumn.cs
[class-columns-boolean]: ./src/Queries.Core/Parts/Columns/BooleanColumn.cs
[class-columns-string]: ./src/Queries.Core/Parts/Columns/StringColumn.cs
[class-columns-datetime]: ./src/Queries.Core/Parts/Columns/DateTimeColumn.cs
[class-columns-field]: ./src/Queries.Core/Parts/Columns/FieldColumn.cs