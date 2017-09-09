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
    public class CompositeWhereClause : IWhereClause
    {
        /// <summary>
        /// Logical operator used between each <see cref="Clauses"/>' item.
        /// </summary>
        public ClauseLogic Logic { get; set; }
        /// <summary>
        /// <see cref="IWhereClause"/> instances that are combined
        /// </summary>
        public IEnumerable<IWhereClause> Clauses { get; set; }

        /// <summary>
        /// Builds a new <see cref="CompositeWhereClause"/> instance.
        /// </summary>
        public CompositeWhereClause() => Clauses = Enumerable.Empty<IWhereClause>();
    }
}