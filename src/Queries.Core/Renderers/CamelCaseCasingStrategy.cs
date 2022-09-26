using System;

namespace Queries.Core.Renderers;

/// <summary>
/// A casing strategy that change casing for each fieldcolumn's name to conform to <see href="https://en.wikipedia.org/wiki/Camel_case">camel casing</see>.
/// </summary>
public sealed class CamelCaseCasingStrategy : FieldnameCasingStrategy
{
    ///<inheritdoc/>
    public override string Handle(string fieldName) => fieldName.ToCamelCase();
}
