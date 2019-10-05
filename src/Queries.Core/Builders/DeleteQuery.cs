using Queries.Core.Attributes;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;

namespace Queries.Core.Builders
{
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
        /// <returns></returns>
        public IBuild<DeleteQuery> Where(IWhereClause clause)
        {
            Criteria = clause;

            return this;
        }

        public IBuild<DeleteQuery> Where(FieldColumn column, ClauseOperator @operator, ColumnBase columnBase) =>
            Where(new WhereClause(column, @operator, columnBase));

        public DeleteQuery Build() => this;

        public override bool Equals(object obj) => Equals(obj as DeleteQuery);

        public bool Equals(DeleteQuery other) => Table.Equals(other?.Table) && Equals(Criteria, other?.Criteria);

#if !NETSTANDARD2_1
        public override int GetHashCode() => (Table, Criteria).GetHashCode();
#else
        public override int GetHashCode() => HashCode.Combine(Table, Criteria);
#endif
    }
}