namespace Queries.Parts.Joins
{
    public class InnerJoin : JoinBase
    {
        public InnerJoin(Table table, IClause @on) : base(JoinType.InnerJoin, table, @on)
        {}
    }
}