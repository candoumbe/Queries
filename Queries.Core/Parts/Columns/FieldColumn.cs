using System;

namespace Queries.Core.Parts.Columns
{
    public class FieldColumn : ColumnBase, INamable, IAliasable<FieldColumn>
    {
        public string Name { get; }

        private string _alias;

        public string Alias => _alias;


        internal FieldColumn(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} cannot be null");
            }
            Name = columnName;
        }

        public FieldColumn As(string alias)
        {
            _alias = alias;

            return this;
        }
    }
}