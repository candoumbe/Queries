using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Clauses;

/// <summary>
/// <para>
/// Allows to combine several <see cref="IHavingClause"/> instances into a single instance.
/// </para>
/// Provides <see cref="Logic"/>
/// </summary>
/// <remarks>
/// Clauses are combined using <see cref="ClauseLogic.And"/> by default.
/// </remarks>
public class CompositeHavingClause : IHavingClause
{
    /// <summary>
    /// Logical operator used between each <see cref="Clauses"/>' item.
    /// </summary>
    public ClauseLogic Logic { get; set; }

    /// <summary>
    /// <see cref="IHavingClause"/>s that are combined
    /// </summary>
    public IEnumerable<IHavingClause> Clauses { get; set; }

    /// <summary>
    /// Builds a new <see cref="CompositeHavingClause"/> instance.
    /// </summary>
    public CompositeHavingClause() => Clauses = Enumerable.Empty<IHavingClause>();

    /// <inheritdoc/>
    public IHavingClause Clone() => new CompositeHavingClause
    {
        Logic = Logic,
        Clauses = Clauses.Select(x => x.Clone()).ToArray()
    };
}