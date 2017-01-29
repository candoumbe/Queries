namespace Queries.Core.Parts.Joins
{
    public class CrossJoin : JoinBase
    {
        public CrossJoin(Table table)
            : base(JoinType.CrossJoin, table, null)
        { }
    }
}