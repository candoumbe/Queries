using System;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// A casing strategy that change casing for each fieldcolumn's name to conform to camel casing <see cref="https://en.wikipedia.org/wiki/Snake_case"/>
    /// </summary>
    public sealed class SnakeCaseCasingStrategy : FieldnameCasingStrategy
    {
        public override string Handle(string fieldName) => fieldName.ToSnakeCase();
    }
}
