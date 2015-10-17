using System;

namespace Queries.Core.Builders
{
    public class TruncateQuery : IDataManipulationQuery
    {
        /// <summary>
        /// Gets or sets the name of the table to truncate.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        public TruncateQuery(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), $"{nameof(tableName)} cannot be null or empty nor whitespace only");
            }
            Name = tableName;
        }
    }
}