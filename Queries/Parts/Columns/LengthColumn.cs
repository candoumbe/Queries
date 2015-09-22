namespace Queries.Parts.Columns
{
    public class LengthColumn : IAliasable<LengthColumn>, IFunctionColumn
    {
        public ColumnBase Column { get; private set; }
        private string _alias;

        public string Alias => _alias;

        public LengthColumn(FieldColumn column)
        {
            Column = column;
        }
        
        public LengthColumn(StringColumn column)
        {
            Column = column;
        }

        public LengthColumn As(string alias)
        {
            _alias = alias;

            return this;
        }
    }
}
