using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// <para>
    /// Allows to combine several <see cref="IWhereClause"/> instances into a single instance.
    /// </para>
    /// Provides <see cref="Logic"/>
    /// </summary>
    /// <remarks>
    /// Clauses are combined using <see cref="ClauseLogic.And"/> by default.
    /// </remarks>
    public class CompositeWhereClause : IWhereClause, IEquatable<CompositeWhereClause>
    {
        /// <summary>
        /// Logical operator used between each <see cref="Clauses"/>' item.
        /// </summary>
        public ClauseLogic Logic { get; set; }
        /// <summary>
        /// <see cref="IWhereClause"/> instances that are combined
        /// </summary>
        public IEnumerable<IWhereClause> Clauses
        {
            get => _clauses ?? Enumerable.Empty<IWhereClause>();
            set => _clauses = value ?? Enumerable.Empty<IWhereClause>();
        }

        private IEnumerable<IWhereClause> _clauses;

        /// <summary>
        /// Builds a new <see cref="CompositeWhereClause"/> instance.
        /// </summary>
        public CompositeWhereClause() => _clauses = Enumerable.Empty<IWhereClause>();

        public override bool Equals(object obj) => Equals(obj as CompositeWhereClause);
        public bool Equals(CompositeWhereClause other) =>
            other != null
            && Logic == other.Logic
            && Clauses.SequenceEqual(other.Clauses);

        public override int GetHashCode()
        {
            int hashCode = 411513146;
            hashCode = (hashCode * -1521134295) + Logic.GetHashCode();
            hashCode = (hashCode * -1521134295) + EqualityComparer<IEnumerable<IWhereClause>>.Default.GetHashCode(Clauses);
            return hashCode;
        }


        public IWhereClause Clone() => new CompositeWhereClause
        {
            Logic = Logic,
            Clauses = Clauses.Select(x => x.Clone()).ToList()
        };
    }
}