using System;
using Queries.Core.Builders.Fluent;
using Queries.Core.Extensions;
using Queries.Core.Parts;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A query to create a new collection of data from a <see cref="SelectQuery"/>.
    /// </summary>
    public class SelectIntoQuery : SelectQueryBase, IBuildableQuery<SelectIntoQuery>
    {
       
        /// <summary>
        /// Where to insert data.
        /// </summary>
        public Table Destination { get; set; }

        /// <summary>
        /// where to gather data from
        /// </summary>
        public ITable Source { get; set; }


        /// <summary>
        /// Builds a new <see cref="SelectIntoQuery"/> instance.
        /// </summary>
        /// <param name="destination"></param>
        public SelectIntoQuery(string destination) : this(destination?.Table())
        {
            
        }

        public SelectIntoQuery(Table table)
        {
            Destination = table ?? throw new ArgumentNullException(nameof(table), "table cannot be null");
        }

        public IBuildableQuery<SelectIntoQuery> From(ITable select)
        {
            Source = select;
            return this;
        }


        

        public SelectIntoQuery Build() => this;
    }

}