namespace Queries.Parts.Joins
{
    public class CrossJoin : JoinBase
    {
        public CrossJoin(TableTerm table)
            : base(JoinType.CrossJoin, table, null)
        { }
    }
}