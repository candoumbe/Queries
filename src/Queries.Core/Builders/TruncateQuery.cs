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
        /// Name of the collection to truncate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Builds a new <see cref="TruncateQuery"/>
        /// </summary>
        /// <param name="collection">Name of the table to truncate</param>
        public TruncateQuery(string collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), $"{nameof(collection)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentOutOfRangeException(nameof(collection), collection, $"{nameof(collection)} cannot be empty or whitespace only");
            }
            Name = collection;
        }

        public override bool Equals(object obj) => Equals(obj as TruncateQuery);
        public bool Equals(TruncateQuery other) => other != null && Name == other.Name;
        public override int GetHashCode() => 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
    }
}