using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Expression to use in a `CASE` statement
    /// </summary>
    public class WhenExpression : IEquatable<WhenExpression>
    {
        /// <summary>
        /// Criterion of the expression
        /// </summary>
        public WhereClause Criterion { get; }

        /// <summary>
        /// Value to use when <see cref="Criterion"/> is <c>false</c>.
        /// </summary>
        public ColumnBase ThenValue { get; set; }


        /// <summary>
        /// Builds a new <see cref="WhenExpression"/> instance.
        /// </summary>
        /// <param name="criterion">criterion</param>
        /// <param name="then">value to output when <paramref name="criterion"/> returns <c>true</c></param>
        /// <exception cref="ArgumentNullException"><paramref name="criterion"/> is <c>null</c></exception>
        public WhenExpression(WhereClause criterion, ColumnBase then)
        {
            Criterion = criterion ?? throw new ArgumentNullException(nameof(criterion));
            ThenValue = then;
        }

        public override bool Equals(object obj) => Equals(obj as WhenExpression);
        public bool Equals(WhenExpression other) => other != null 
            && EqualityComparer<WhereClause>.Default.Equals(Criterion, other.Criterion) && EqualityComparer<ColumnBase>.Default.Equals(ThenValue, other.ThenValue);

        public override int GetHashCode()
        {
            int hashCode = -493889257;
            hashCode = hashCode * -1521134295 + EqualityComparer<WhereClause>.Default.GetHashCode(Criterion);
            hashCode = hashCode * -1521134295 + EqualityComparer<ColumnBase>.Default.GetHashCode(ThenValue);
            return hashCode;
        }

        public static bool operator ==(WhenExpression expression1, WhenExpression expression2)
        {
            return EqualityComparer<WhenExpression>.Default.Equals(expression1, expression2);
        }

        public static bool operator !=(WhenExpression expression1, WhenExpression expression2)
        {
            return !(expression1 == expression2);
        }
    }
}
