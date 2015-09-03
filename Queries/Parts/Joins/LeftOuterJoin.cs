namespace Queries.Parts.Joins
{
    public class LeftOuterJoin : JoinBase
    {
        public LeftOuterJoin(TableTerm table, IClause @on)
            : base(JoinType.LeftOuterJoin, table, @on)
        { }
    }
}