using System;
using Queries.Core.Builders.Fluent;
#if !SYSTEM_TEXT_JSON
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using Queries.Core.Attributes;

namespace Queries.Core.Builders;

/// <summary>
/// Query which result in creating a View.
/// </summary>
[DataManipulationLanguage]
#if !SYSTEM_TEXT_JSON
[JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
#endif
public class CreateViewQuery : IBuild<CreateViewQuery>, IEquatable<CreateViewQuery>
{
    /// <summary>
    /// Name of the view
    /// </summary>
    public string ViewName { get; }

    /// <summary>
    /// <see cref="Builders.SelectQuery"/> the view will be built from
    /// </summary>
    public SelectQuery SelectQuery { get; private set; }

    /// <summary>
    /// Builds a new <see cref="CreateViewQuery"/> instance.
    /// </summary>
    /// <param name="viewName">Name of the view</param>
    public CreateViewQuery(string viewName)
    {
        if (viewName == null)
        {
            throw new ArgumentNullException(nameof(viewName));
        }

        if (viewName.Trim().Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewName));
        }

        ViewName = viewName;
    }

    /// <summary>
    /// Defines the body of the current instance.
    /// </summary>
    /// <param name="select"></param>
    /// <returns><see cref="IBuild{CreateViewQuery}"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public IBuild<CreateViewQuery> As(SelectQuery select)
    {
        SelectQuery = select ?? throw new ArgumentNullException(nameof(select));

        return this;
    }

    ///<inheritdoc/>
    public CreateViewQuery Build() => this;

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as CreateViewQuery);

    ///<inheritdoc/>
    public bool Equals(CreateViewQuery other) => other != null && ViewName == other.ViewName && EqualityComparer<SelectQuery>.Default.Equals(SelectQuery, other.SelectQuery);

    ///<inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = -954091970;
        hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(ViewName);
        return (hashCode * -1521134295) + EqualityComparer<SelectQuery>.Default.GetHashCode(SelectQuery);
    }
}
