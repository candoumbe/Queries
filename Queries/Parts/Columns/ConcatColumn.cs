using System;
using System.Collections.Generic;

namespace Queries.Parts.Columns
{
    public class ConcatColumn : IAliasable<ConcatColumn>, IFunctionColumn
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
                throw new ArgumentOutOfRangeException(nameof(columns), $"{nameof(columns)} must be at least two columns to concatenate");
            }
            Columns = columns;
        }

        private string _alias;

        public string Alias => _alias;

        public ConcatColumn As(string alias)
        {
            _alias = alias;

            return this;
        }
    }
}