namespace Queries.Parts.Joins
{
    public interface IJoin
    {
        JoinType JoinType { get; }
        Table Table { get; }
        IClause On { get; }
    }
}
