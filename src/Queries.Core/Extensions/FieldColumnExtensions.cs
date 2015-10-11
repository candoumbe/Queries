using System;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Extensions
{
    public static class FieldColumnExtensions
    {
        public static UpdateFieldValue EqualTo(this FieldColumn destination, ColumnBase source)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }


            return new UpdateFieldValue(destination, source);
            
        }


        public static WhereClause IsNull(this FieldColumn column) => new WhereClause(column, ClauseOperator.IsNull);


        public static WhereClause IsNotNull(this FieldColumn column) => new WhereClause(column, ClauseOperator.IsNotNull);
      
        public static WhereClause LessThan(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.LessThan, constraint);

        public static WhereClause GreaterThan(this FieldColumn column, ColumnBase constraint) => new WhereClause(column, ClauseOperator.GreaterThan, constraint);

    }
}