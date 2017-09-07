using Newtonsoft.Json;
using Queries.Core.Attributes;
using System;
using System.Collections.Generic;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to delete data from a table.
    /// </summary>
    [JsonObject]
    [DataManipulationLanguage]
    public class TruncateQuery : IEquatable<TruncateQuery>, IQuery
    {
        /// <summary>
        /// Name of the table to truncate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Builds a new <see cref="TruncateQuery"/>
        /// </summary>
        /// <param name="tableName">Name of the table to truncate</param>
        public TruncateQuery(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), $"{nameof(tableName)} cannot be empty or whitespace only");
            }
            Name = tableName;
        }

        public override bool Equals(object obj) => Equals(obj as TruncateQuery);
        public bool Equals(TruncateQuery other) => other != null && Name == other.Name;
        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }
}