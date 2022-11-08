using System.Collections.Generic;

using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;

using System;

using Queries.Core.Attributes;

using System.Linq;

#if !SYSTEM_TEXT_JSON
using Newtonsoft.Json;
#endif

namespace Queries.Core.Builders;

/// <summary>
/// A query to update a table
/// </summary>
#if !SYSTEM_TEXT_JSON
[JsonObject]
#endif
[DataManipulationLanguage]
public class UpdateQuery : IQuery, IEquatable<UpdateQuery>
{
    /// <summary>
    /// Table to update
    /// </summary>
    public Table Table { get; }

    /// <summary>
    /// Collection of values
    /// </summary>
    public IList<UpdateFieldValue> Values { get; private set; }

    /// <summary>
    /// Criteria associated with the current instance.
    /// </summary>
    public IWhereClause Criteria { get; set; }

    /// <summary>
    /// Builds a new <see cref="UpdateQuery"/> instance.
    /// </summary>
    /// <param name="tableName"></param>
    /// <exception cref="ArgumentNullException"><paramref name="tableName"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tableName"/> is empty or only contains whitespaces.</exception>
    public UpdateQuery(string tableName)
    {
        if (tableName is null)
        {
            throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} cannot be null");
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentOutOfRangeException(nameof(tableName), tableName, $"{nameof(tableName)} cannot be empty or whitespace");
        }

        Table = tableName.Table();
        Values = new List<UpdateFieldValue>();
    }

    /// <summary>
    /// Builds a new <see cref="UpdateQuery"/> instance.
    /// </summary>
    /// <param name="table"></param>
    public UpdateQuery(Table table) : this(table?.Name)
    {
    }

    /// <summary>
    /// Defines the <c>SET</c> part of the current instance.
    /// </summary>
    /// <param name="newValues"></param>
    /// <returns>the current instance for further processing</returns>
    public UpdateQuery Set(params UpdateFieldValue[] newValues)
    {
        Values = newValues;
        return this;
    }

    /// <summary>
    /// Adds the specified <see cref="IWhereClause"/> to the current instance
    /// </summary>
    /// <param name="clause"></param>
    /// <returns>The current <see cref="UpdateQuery"/> for further processing</returns>
    public UpdateQuery Where(IWhereClause clause)
    {
        Criteria = clause;
        return this;
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as UpdateQuery);

    ///<inheritdoc/>
    public bool Equals(UpdateQuery other) => other is not null
                                             && ((Table == null && other.Table == null) || Table.Equals(other.Table))
                                             && Values.SequenceEqual(other.Values)
                                             && ((Criteria == null && other.Criteria == null) || Criteria.Equals(other.Criteria));

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = -1291674402;
        hashCode = (hashCode * -1521134295) + EqualityComparer<Table>.Default.GetHashCode(Table);
        hashCode = (hashCode * -1521134295) + EqualityComparer<IList<UpdateFieldValue>>.Default.GetHashCode(Values);
        return (hashCode * -1521134295) + EqualityComparer<IWhereClause>.Default.GetHashCode(Criteria);
    }

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();
}