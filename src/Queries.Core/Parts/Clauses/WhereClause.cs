﻿using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// a criterion to apply to a <see cref="Column"/>.
    /// </summary>
    public class WhereClause : IWhereClause, IClause<IColumn>, IEquatable<WhereClause>
    {
        public Guid UniqueId { get; }
        public IColumn Column { get; }
        public ClauseOperator Operator{ get; }
        public ColumnBase Constraint { get; set; }

        public bool IsParameterized { get; }

        /// <summary>
        /// Builds a new <see cref="WhereClause"/> instance.
        /// </summary>
        /// <param name="column"><see cref="IColumn"/> where to apply the clase onto</param>
        /// <param name="operator"><see cref="ClauseOperator"/> to apply</param>
        /// <param name="constraint">constraint to apply to <paramref name="column"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is <c>null</c>.</exception>
        public WhereClause(IColumn column, ClauseOperator @operator, ColumnBase constraint = null)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            UniqueId = Guid.NewGuid();
            Operator = @operator;
            if (@operator != ClauseOperator.IsNull && @operator != ClauseOperator.IsNotNull)
            {
                Constraint = constraint;
                IsParameterized = constraint is LiteralColumn lc && lc.Value != null; 
            }
        }

        public override bool Equals(object obj) => Equals(obj as WhereClause);
        public bool Equals(WhereClause other) => 
            other != null 
            && Column.Equals(other.Column)
            && Operator == other.Operator 
            && (Constraint == null && other.Constraint == null || Constraint.Equals(other.Constraint));

        public override int GetHashCode()
        {
            int hashCode = -300605098;
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Column);
            hashCode = hashCode * -1521134295 + Operator.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ColumnBase>.Default.GetHashCode(Constraint);
            return hashCode;
        }
    }
}
