﻿using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Renderers.Postgres.Parts.Columns;

namespace Queries.Renderers.Postgres
{
    public static class JsonFieldColumnExtensions
    {
        /// <summary>
        /// Builds a <see cref="WhereClause"/> instance.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="constraint"></param>
        /// <returns></returns>
        public static WhereClause EqualTo(this JsonFieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.EqualTo, constraint);
    }
}
