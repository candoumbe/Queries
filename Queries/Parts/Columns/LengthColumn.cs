using System.CodeDom;

namespace Queries.Parts.Columns
{
    public class LengthColumn : IFunctionColumn, IAliasable
    {
        public ColumnBase Column { get; private set; }
        public string Alias { get; set; }

        public LengthColumn(FieldColumn column, string alias = null)
        {
            Column = column;
        }
        
        public LengthColumn(StringColumn column, string alias = null)
        {
            Column = column;
        }

    }
}
