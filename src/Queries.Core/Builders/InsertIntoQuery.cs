using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    public class InsertIntoQuery : IDataManipulationQuery, IInsertIntoQuery<InsertIntoQuery>, IBuildableQuery<InsertIntoQuery>
    {
        public IInsertable InsertedValue { get; set; }

        public string TableName { get; }


        /// <summary>
        /// Creates a new <see cref="InsertIntoQuery"/>
        /// </summary>
        /// <param name="tableName">name of the table the INSERT INTO will be made for</param>
        public InsertIntoQuery(string tableName)
        {
            TableName = tableName;
        }


        public IBuildableQuery<InsertIntoQuery> Values(SelectQuery values)
        {
            InsertedValue = values;

            return this;
        }

        
        public IBuildableQuery<InsertIntoQuery> Values(params InsertedValue[] values)
        {
            InsertedValues insertedValues = new InsertedValues();
            foreach (InsertedValue insertedValue in values)
            {
                insertedValues.Add(insertedValue);
            }

            InsertedValue = insertedValues;

            return this;
        }

        public InsertIntoQuery Build() => this;

        
    }
}
