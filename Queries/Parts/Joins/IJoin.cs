namespace Queries.Parts.Joins
{
    public interface IJoin
    {
        JoinType JoinType { get; }
        TableTerm Table { get; }
        IClause On { get; }
    }
}
