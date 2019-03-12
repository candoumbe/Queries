using System;
using Queries.Core.Builders;
using System.Collections.Generic;
using Newtonsoft.Json;
using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Parts.Columns
{
    [JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
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

        public override bool Equals(object obj) =>
            ReferenceEquals(obj, this) || (obj is FieldColumn fc && this.Equals(fc));

        public bool Equals(FieldColumn other) => other != null && Name == other.Name && Alias == other.Alias;

        public override int GetHashCode()
        {
            int hashCode = 1124293869;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(Alias);
            return hashCode;
        }

        public override string ToString() => SerializeObject(this);

        /// <summary>
        /// Performs a deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override IColumn Clone() => new FieldColumn(Name);
    }
}