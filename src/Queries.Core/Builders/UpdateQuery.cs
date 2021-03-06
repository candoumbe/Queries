using System.Collections.Generic;
using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using System;
using Queries.Core.Attributes;
using System.Linq;
using Newtonsoft.Json;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to update a table
    /// </summary>
    [JsonObject]
    [DataManipulationLanguage]
    public class UpdateQuery : IQuery, IEquatable<UpdateQuery>
    {
        /// <summary>
        /// Table to update
        /// </summary>
        public Table Table { get; }
        public IList<UpdateFieldValue> Values { get; private set; }
        public IWhereClause Criteria { get; set; }

        public UpdateQuery(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName), $"{nameof(tableName)} cannot be null");
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentOutOfRangeException(nameof(tableName), tableName, $"{nameof(tableName)} cannot be empty or whitespace");
            }

            Table = tableName.Table();
            Values = new List<UpdateFieldValue>();
        }

        public UpdateQuery(Table table) : this(table?.Name)
        {

        }

        public UpdateQuery Set(params UpdateFieldValue[] newValues)
        {
            Values = newValues;
            return this;
        }

        public UpdateQuery Where(IWhereClause clause)
        {
            Criteria = clause;
            return this;
        }

        public override bool Equals(object obj) => Equals(obj as UpdateQuery);

        public bool Equals(UpdateQuery other) => other != null
                && ((Table == null && other.Table == null) || Table.Equals(other.Table))
                && Values.SequenceEqual(other.Values)
                && ((Criteria == null && other.Criteria == null) || Criteria.Equals(other.Criteria));

        public override int GetHashCode()
        {
            int hashCode = -1291674402;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Table>.Default.GetHashCode(Table);
            hashCode = (hashCode * -1521134295) + EqualityComparer<IList<UpdateFieldValue>>.Default.GetHashCode(Values);
            return (hashCode * -1521134295) + EqualityComparer<IWhereClause>.Default.GetHashCode(Criteria);
        }

        public override string ToString() => this.Jsonify();
    }
}