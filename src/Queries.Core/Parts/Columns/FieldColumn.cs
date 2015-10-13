using System;
using Queries.Core.Builders;

namespace Queries.Core.Parts.Columns
{
    public class FieldColumn : ColumnBase, INamable, IAliasable<FieldColumn>, IInsertable
    {
        public string Name { get; }

        private string _alias;

        public string Alias => _alias;


        public FieldColumn(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} cannot be null");
            }
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentOutOfRangeException(nameof(columnName), $"{nameof(columnName)} cannot be empty or whitespace only");
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