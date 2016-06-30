using System;
using Queries.Core.Builders.Fluent;
using Queries.Core.Extensions;
using Queries.Core.Parts;

namespace Queries.Core.Builders
{
    public class SelectIntoQuery : SelectQueryBase, IBuildableQuery<SelectIntoQuery>
    {
       
        public Table Destination { get; set; }

        public ITable Source { get; set; }



        public SelectIntoQuery(string destination) : this(destination?.Table())
        {
            
        }

        public SelectIntoQuery(Table table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table), "table cannot be null");
            }
            
           Destination = table;
        }

        public IBuildableQuery<SelectIntoQuery> From(ITable select)
        {
            Source = select;
            return this;
        }


        

        public SelectIntoQuery Build() => this;
    }

}