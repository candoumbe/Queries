namespace Queries.Parts.Joins
{
    public class RightOuterJoin : JoinBase
    {
        public RightOuterJoin(TableTerm table, IClause @on)
            : base(JoinType.RightOuterJoin, table, @on)
        { }
    }
}