using Queries.Core.Attributes;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

using System;

namespace Queries.Core.Builders;

/// <summary>
/// Query that produces a DELETE statement.
/// </summary>
/// <remarks>
/// 
/// </remarks>
[DataManipulationLanguage]
public class DeleteQuery : IBuild<DeleteQuery>, IEquatable<DeleteQuery>
{
    /// <summary>
    /// Name of the table where to delete data from
    /// </summary>
    public string Table { get; }

    /// <summary>
    /// Criteria for data to delete from <see cref="Table"/>.
    /// </summary>
    public IWhereClause Criteria { get; private set; }

    /// <summary>
    /// Builds a new <see cref="DeleteQuery"/> instance.
    /// </summary>
    /// <param name="tableName">Name of the table to delete data from</param>
    public DeleteQuery(string tableName) => Table = tableName ?? throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} cannot be null");

    /// <summary>
    /// Defines the <see cref="Criteria"/>
    /// </summary>
    /// <param name="clause"></param>
    /// <returns>the current <see cref="DeleteQuery"/> instance for further processing</returns>
    public IBuild<DeleteQuery> Where(IWhereClause clause)
    {
        Criteria = clause;

        return this;
    }

    /// <inheritdoc cref="Where(IWhereClause)"/>
    public IBuild<DeleteQuery> Where(FieldColumn column, ClauseOperator @operator, ColumnBase columnBase) =>
        Where(new WhereClause(column, @operator, columnBase));

    ///<inheritdoc/>
    public DeleteQuery Build() => this;

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as DeleteQuery);

    ///<inheritdoc/>
    public bool Equals(DeleteQuery other) => Table.Equals(other?.Table) && Equals(Criteria, other?.Criteria);

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode() => (Table, Criteria).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Table, Criteria);
#endif
}