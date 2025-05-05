## Parameterized queries

Renderers provided [Queries] automatically takes care of literal values when rendering any query.

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
This should prevent any SQL injection. But this can become expensive when running the same query again and again as the database engine cannot optimize 
nor reuse execution plans from previous runs. 

The [recommended approach](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/configuring-parameters-and-parameter-data-types) is to use parametrized queries. [Queries] can help you in that area as well by leveraging `CollectVariableVisitor` as shown in the following example (SQL Server)

```csharp
// 1. Create your query
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

// 2. Create an instance of [CollectVariableVisitor]
CollectVariableVisitor visitor = new CollectVariableVisitor();

// 3. Use the visitor instance to collect variables
visitor.Visit(query); // <-- This will rewrite the query and replace the "variable" part with a true [Variable] 

// 4. The query instance was rewritten and can safely be used with ADO.NET
using (SqlConnection connection = new(connectionString))
{
    // Create the command and set its properties.
    SqlCommand command = new()
    {
        Connection = connection,
        CommandText = query.ForSqlServer(), // <-- render the query as string for SQL Server.
    };

    // loop through all variables gathered by the visitor instance whilst visiting the query
    foreach(Variable variable in visitor.Variables)
    {
        SqlParameter parameter = new()
        {
            ParameterName = variable.Name,
            SqlDbType = MapVariableTypeToSqlType(variable.Type),
            Value = variable.Value
        };
        
        // Add the parameter to the Parameters collection.
        command.Parameters.Add(parameter);
    }
 
    // 5. Run the sql commmand as usual
    
}
```

You can write your queries as usual and use a [CollectVariableVisitor] to extract variables from it.
[CollectVariableVisitor] can be used with almost every [IQuery] implementation: in essence, it rewrites any query it visits
and replaces any encountered [Literal] by an equivalent [Variable].

An even easier approach is to "compile" your query. The previous example can easily be rewritten as follows :
```csharp
// 1. Create your query
IQuery query = Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field()).As("Fullname"))
    .From("members".Table())
    .Where("Age".Field().GreaterThanOrEqualTo(18));

// 2. Compile the query
CompiledQuery compiledQuery = query.CompileForXXXXX();

// 4. The query instance was rewritten and can safely be used with ADO.NET
using (SqlConnection connection = new(connectionString))
{
    // Create the command and set its properties.
    SqlCommand command = new()
    {
        Connection = connection,
        CommandText = compiledQuery.Statement, // <-- render the query as string for SQL Server.
    };

    // loop through all variables gathered by the visitor instance whilst visiting the query
    foreach(Variable variable in compiledQuery.Variables)
    {
        SqlParameter parameter = new()
        {
            ParameterName = variable.Name,
            SqlDbType = MapVariableTypeToSqlType(variable.Type),
            Value = variable.Value
        };
        
        // Add the parameter to the Parameters collection.
        command.Parameters.Add(parameter);
    }
 
    // 5. Run the sql commmand as usual
    
}
```

[Queries]:https://nuget.org/packages/queries
[Variable]:../src/Queries.Core/Parts/Clauses/Variable.cs
[CollectVariableVisitor]:../src/Queries.Core/Builders/CollectVariableVisitor.cs 
[IQuery]:../src/Queries.Core/IQuery.cs
[Literal]:../src/Queries.Core/Parts/Columns/Literal.cs