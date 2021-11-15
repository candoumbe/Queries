using Queries.Core.Parts.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Model a CASE statement
    /// </summary>
    public class CasesColumn : ColumnBase, IAliasable<CasesColumn>, IEquatable<CasesColumn>
    {
        /// <summary>
        /// `WHEN` expressions
        /// </summary>
        public IEnumerable<WhenExpression> Cases { get; }

        ///<inheritdoc/>
        public string Alias { get; private set; }

        
        public ColumnBase Default { get; set; }

        /// <summary>
        /// Builds a new <see cref="CasesColumn"/> instances
        /// </summary>
        /// <param name="cases"></param>
        /// <exception cref="ArgumentNullException"><paramref name="cases"/> is <c>null</c></exception>
        public CasesColumn(IEnumerable<WhenExpression> cases)
        {
            Cases = cases ?? throw new ArgumentNullException(nameof(cases));
        }

        ///<inheritdoc/>
        public override IColumn Clone() => new CasesColumn(Cases);

        ///<inheritdoc/>
        public override bool Equals(ColumnBase other)
        {
            bool equals = false;

            return equals;
        }

        ///<inheritdoc/>
        public CasesColumn As(string alias)
        {
            Alias = alias;
            return this;
        }

        ///<inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as CasesColumn);

        ///<inheritdoc/>
        public bool Equals(CasesColumn other)
        {
            bool equals = other != null
            && Cases.Count() == other.Cases.Count() && !Cases.Except(other.Cases).Any()
            && Alias == other.Alias;

            return equals;
        }

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1689548013;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<WhenExpression>>.Default.GetHashCode(Cases);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alias);
            return hashCode;
        }

        /// <summary>
        /// Sets a value to use when none of the <see cref="Cases"/> matched
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public CasesColumn Else(ColumnBase column)
        {
            Default = column;

            return this;
        }

        ///<inheritdoc/>
        public static bool operator ==(CasesColumn column1, CasesColumn column2)
        {
            return EqualityComparer<CasesColumn>.Default.Equals(column1, column2);
        }

        ///<inheritdoc/>
        public static bool operator !=(CasesColumn column1, CasesColumn column2)
        {
            return !(column1 == column2);
        }
    }
}
