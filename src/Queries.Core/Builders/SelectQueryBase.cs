using System.Collections.Generic;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Joins;
using Queries.Core.Parts.Sorting;
using Queries.Core.Attributes;

namespace Queries.Core.Builders;

/// <summary>
/// Base class for queries that select data
/// </summary>
[DataManipulationLanguage]
public abstract class SelectQueryBase : IInsertable, IQuery
{
    /// <summary>
    /// Gets <see cref="IColumn"/>s that were selected
    /// </summary>
    public IList<IColumn> Columns { get; protected set; }

    /// <summary>
    /// Gets <see cref="IWhereClause"/>s that were applied to the current instance
    /// </summary>
    public IWhereClause WhereCriteria { get; protected set; }

    /// <summary>
    /// Gets <see cref="IHavingClause"/>s that were applied to the current instance
    /// </summary>
    public IHavingClause HavingCriteria { get; protected set; }

    /// <summary>
    /// Gets <see cref="IJoin"/>s that were applied to the current instance
    /// </summary>
    public IList<IJoin> Joins { get; protected set; }

    /// <summary>
    /// Gets <see cref="IOrder"/>s that were applied to the current instance
    /// </summary>
    public IList<IOrder> Orders { get; protected set; }

    /// <summary>
    /// Builds a new <see cref="SelectQueryBase"/> instance.
    /// </summary>
    protected SelectQueryBase()
    {
        Columns = new List<IColumn>();
        Joins = new List<IJoin>();
        Orders = new List<IOrder>();
    }
}