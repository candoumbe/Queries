using System;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// A casing strategy that change casing for each fieldcolumn's name to conform to camel casing <see cref="https://en.wikipedia.org/wiki/Camel_case"/>
    /// </summary>
    public sealed class CamelCaseCasingStrategy : FieldnameCasingStrategy
    {
        public override string Handle(string fieldName) => fieldName.ToCamelCase();
    }
}
