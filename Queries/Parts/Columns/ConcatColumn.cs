using System;
using System.Collections.Generic;

namespace Queries.Parts.Columns
{
    public class ConcatColumn : IFunctionColumn, IAliasable
    {
        public IList<IColumn> Columns { get; private set; }

        public ConcatColumn(params IColumn[] columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException();
            }
            if (columns.Length < 2)
            {
                throw new ArgumentOutOfRangeException("columns", "must be at least two columns to concatenate");
            }
            Columns = columns;
        }

        public string Alias { get; set; }
    }
}