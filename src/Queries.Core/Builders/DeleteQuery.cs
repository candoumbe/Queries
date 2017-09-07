using Queries.Core.Attributes;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Query that produce a DELETE statement.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [DataManipulationLanguage]
    public class DeleteQuery : IBuildableQuery<DeleteQuery>
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
        /// Builds a new <see cref="DeleteQuery"/> instance
        /// </summary>
        /// <param name="tableName">Name of the table to delete data from</param>
        public DeleteQuery(string tableName)
        {
            Table = tableName ?? throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} cannot be null");
        }

        /// <summary>
        /// Defines the <see cref="Criteria"/>
        /// </summary>
        /// <param name="clause"></param>
        /// <returns></returns>
        public IBuildableQuery<DeleteQuery> Where(IWhereClause clause)
        {
            Criteria = clause;

            return this;
        }


        public IBuildableQuery<DeleteQuery> Where(FieldColumn column, ClauseOperator @operator, ColumnBase columnBase) =>
            Where(new WhereClause(column, @operator, columnBase));


        public DeleteQuery Build() => this;

    }
}