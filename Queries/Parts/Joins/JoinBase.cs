namespace Queries.Parts.Joins
{
    public abstract class JoinBase : IJoin
    {
        public JoinType JoinType { get; set; }
        public TableTerm Table { get; set; }
        public IClause On { get; set; }

        protected JoinBase(JoinType joinType, TableTerm table, IClause on)
        {
            JoinType = joinType;
            Table = table;
            On = @on;
        }
    }
}