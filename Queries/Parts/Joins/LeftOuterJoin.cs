namespace Queries.Parts.Joins
{
    public class LeftOuterJoin : JoinBase
    {
        public LeftOuterJoin(Table table, IClause @on)
            : base(JoinType.LeftOuterJoin, table, @on)
        { }
    }
}