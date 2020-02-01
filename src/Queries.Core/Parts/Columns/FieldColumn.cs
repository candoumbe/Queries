using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns
{
    public class FieldColumn : ColumnBase, INamable, IAliasable<FieldColumn>, IInsertable, IEquatable<FieldColumn>
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name { get; set; }

        private string _alias;

        /// <summary>
        /// Alias of the column
        /// </summary>
        public string Alias => _alias;

        public FieldColumn(string columnName)
        {
            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName), $"{nameof(columnName)} cannot be null");
            }
            if (string.IsNullOrWhiteSpace(columnName))
            {
                throw new ArgumentOutOfRangeException(nameof(columnName), columnName, $"{nameof(columnName)} cannot be empty or whitespace only");
            }
            Name = columnName;
        }

        public FieldColumn As(string alias)
        {
            _alias = alias;

            return this;
        }

        public override bool Equals(object obj) => Equals(obj as FieldColumn);

        public bool Equals(FieldColumn other) => (Name, Alias).Equals((other?.Name, other?.Alias));

        public override bool Equals(ColumnBase other) => Equals(other as FieldColumn);

#if !NETSTANDARD2_1
        public override int GetHashCode() => (Name, Alias).GetHashCode();
#else
        public override int GetHashCode() => HashCode.Combine(Name, Alias);
#endif
        public override string ToString() => this.Jsonify();

        /// <summary>
        /// Performs a deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override IColumn Clone() => new FieldColumn(Name);
    }
}