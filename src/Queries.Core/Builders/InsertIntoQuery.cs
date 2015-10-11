using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Queries.Core.Builders.Fluent;

namespace Queries.Core.Builders
{
    public class InsertIntoQuery : IQuery, IInsertIntoQuery<InsertIntoQuery>, IBuildableQuery<InsertIntoQuery>
    {
        /// <summary>
     
        /// </summary>
        /// 
 
        public SelectQuery Query { get; set; }

        public string TableName { get; }

        /// <summary>
        /// Creates a new <see cref="InsertIntoQuery"/>
        /// </summary>
        /// <param name="tableName">name of the table the INSERT INTO will be made for</param>
        /// <param name="columns">  </param>
        public InsertIntoQuery(string tableName)
        {
            TableName = tableName;
            
        }


        public IBuildableQuery<InsertIntoQuery> Values(SelectQuery select)
        {
            Query = select;

            return this;
        }

        public InsertIntoQuery Build() => this;
    }   
}
