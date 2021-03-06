namespace Queries.Core.Builders;

/// <summary>
/// A query to insert data 
/// </summary>
public class InsertIntoQuery : IInsertIntoQuery<InsertIntoQuery>, IBuild<InsertIntoQuery>, IEquatable<InsertIntoQuery>
{
    /// <summary>
    /// Values to insert
    /// </summary>
    public IInsertable InsertedValue { get; private set; }

    /// <summary>
    /// Name of the element where to insert <see cref="InsertedValue"/>
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Creates a new <see cref="InsertIntoQuery"/>
    /// </summary>
    /// <param name="tableName">name of the table the INSERT INTO will be made for</param>
    /// <exception cref="ArgumentNullException">if <paramref name="tableName"/> is <see langword="null" />.</exception>
    public InsertIntoQuery(string tableName) => TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

    /// <summary>
    /// Defines values to insert as a <see cref="SelectQuery"/>.
    /// </summary>
    /// <param name="select">The query from whi</param>
    /// <returns></returns>
    public IBuild<InsertIntoQuery> Values(SelectQuery select)
    {
        InsertedValue = select;

        return this;
    }

    /// <summary>
    /// Defines the VALUES the current query will insert
    /// </summary>
    /// <param name="value">First value to insert</param>
    /// <param name="values">Additional values to insert</param>
    /// <returns></returns>
    public IBuild<InsertIntoQuery> Values(InsertedValue value, params InsertedValue[] values)
    {
        InsertedValues insertedValues = new() { value };
        foreach (InsertedValue insertedValue in values)
        {
            insertedValues.Add(insertedValue);
        }

        InsertedValue = insertedValues;

        return this;
    }

    ///<inheritdoc/>
    public InsertIntoQuery Build() => this;

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as InsertIntoQuery);

    ///<inheritdoc/>
    public bool Equals(InsertIntoQuery other) => other is not null
                                                && TableName == other.TableName
                                                && InsertedValue.Equals(other.InsertedValue);

    ///<inheritdoc/>
    public override int GetHashCode() => (InsertedValue, TableName).GetHashCode();
}
