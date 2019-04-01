using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to insert data 
    /// </summary>
    public class InsertIntoQuery : IInsertIntoQuery<InsertIntoQuery>, IBuild<InsertIntoQuery>, IEquatable<InsertIntoQuery>
    {
        /// <summary>
        /// Values to insert
        /// </summary>
        public IInsertable InsertedValue { get; private set; }

        /// <summary>
        /// Name of the element where to insert <see cref="InsertedValue"/>
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// Creates a new <see cref="InsertIntoQuery"/>
        /// </summary>
        /// <param name="tableName">name of the table the INSERT INTO will be made for</param>
        /// <exception cref="ArgumentNullException">if <paramref name="tableName"/> is <c>null</c>.</exception>
        public InsertIntoQuery(string tableName) => TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

        public IBuild<InsertIntoQuery> Values(SelectQuery select)
        {
            InsertedValue = select;

            return this;
        }

        public IBuild<InsertIntoQuery> Values(InsertedValue value, params InsertedValue[] values)
        {
            InsertedValues insertedValues = new InsertedValues() { value };
            foreach (InsertedValue insertedValue in values)
            {
                insertedValues.Add(insertedValue);
            }

            InsertedValue = insertedValues;

            return this;
        }

        public InsertIntoQuery Build() => this;

        public override bool Equals(object obj) => Equals(obj as InsertIntoQuery);

        public bool Equals(InsertIntoQuery other) =>
            other != null
            && TableName == other.TableName
            && InsertedValue.Equals(other.InsertedValue);

        public override int GetHashCode() => (InsertedValue, TableName).GetHashCode();
    }
}
