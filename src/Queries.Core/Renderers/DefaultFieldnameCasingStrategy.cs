namespace Queries.Core.Renderers
{
    /// <summary>
    /// A strategy that leaves the field name unchanged
    /// </summary>
    public sealed class DefaultFieldnameCasingStrategy : FieldnameCasingStrategy
    {
        public override string Handle(string fieldName) => fieldName;
    }
}
