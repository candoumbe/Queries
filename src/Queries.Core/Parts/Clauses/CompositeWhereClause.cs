using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Clauses;

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
    public ClauseLogic Logic
    {
        get;
#if NET6_0_OR_GREATER
        init;
#else
        set; 
#endif
    }

    /// <summary>
    /// <see cref="IWhereClause"/> instances that are combined
    /// </summary>
    public IEnumerable<IWhereClause> Clauses
    {
        get => _clauses ?? Enumerable.Empty<IWhereClause>();
#if NET6_0_OR_GREATER
        init
#else 
        set
#endif
        => _clauses = value ?? Enumerable.Empty<IWhereClause>();
    }

    private IEnumerable<IWhereClause> _clauses;

    /// <summary>
    /// Builds a new <see cref="CompositeWhereClause"/> instance.
    /// </summary>
    public CompositeWhereClause() => _clauses = Enumerable.Empty<IWhereClause>();

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as CompositeWhereClause);

    ///<inheritdoc/>
    public bool Equals(CompositeWhereClause other) => other is not null
                                                      && Logic == other.Logic
                                                      && Clauses.SequenceEqual(other.Clauses); 

    ///<inheritdoc/>
    public bool Equals(IWhereClause other) => Equals(other as CompositeWhereClause);

    ///<inheritdoc/>
    public override int GetHashCode() => (Logic, Clauses).GetHashCode();


    ///<inheritdoc/>
    public IWhereClause Clone()
        => new CompositeWhereClause
        {
            Logic = Logic,
            Clauses = Clauses.Select(x => x.Clone()).ToArray()
        };

    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();
}