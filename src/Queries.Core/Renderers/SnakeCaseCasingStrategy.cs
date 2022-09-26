using System;

namespace Queries.Core.Renderers;

/// <summary>
/// A casing strategy that change casing for each fieldcolumn's name to conform to <see href="https://en.wikipedia.org/wiki/Snake_case">snake casing</see>
/// </summary>
public sealed class SnakeCaseCasingStrategy : FieldnameCasingStrategy
{
    ///<inheritdoc/>
    public override string Handle(string fieldName) => fieldName.ToSnakeCase();
}
