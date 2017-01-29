using System.Collections.Generic;

namespace Queries.Core.Builders
{
    /// <summary>
    /// An instance of this class represents a batch of queries that will be executed as a whole
    /// </summary>
    public class BatchQuery : IQuery
    {
        public IEnumerable<IQuery> Statements { get;  }
        
        public BatchQuery(params IQuery[] queries)
        {
            Statements = queries;
        }
    }
}
