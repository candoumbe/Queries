namespace Queries.Parts.Joins
{
    public class InnerJoin : JoinBase
    {
        public InnerJoin(TableTerm table, IClause @on) : base(JoinType.InnerJoin, table, @on)
        {}
    }
}