﻿using Queries.Core.Parts.Columns;

namespace Queries.Core.Parts.Functions
{
    public class LengthColumn : IAliasable<LengthColumn>, IFunctionColumn
    {
        public IColumn Column { get; private set; }
        private string _alias;

        public string Alias => _alias;

        internal LengthColumn(IColumn column)
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
