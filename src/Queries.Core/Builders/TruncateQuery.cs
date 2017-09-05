using System;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to delete data from a table.
    /// </summary>
    public class TruncateQuery : IDataManipulationQuery
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
                throw new ArgumentOutOfRangeException(nameof(tableName), $"{nameof(tableName)} cannot be empty");
            }
            Name = tableName;
        }
    }
}