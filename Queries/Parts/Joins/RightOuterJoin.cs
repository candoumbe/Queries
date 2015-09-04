namespace Queries.Parts.Joins
{
    public class RightOuterJoin : JoinBase
    {
        public RightOuterJoin(Table table, IClause @on)
            : base(JoinType.RightOuterJoin, table, @on)
        { }
    }
}