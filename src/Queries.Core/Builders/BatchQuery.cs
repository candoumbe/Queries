using System.Collections.Generic;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Represents many <see cref="IQuery"/>s that will be executed as a whole
    /// </summary>
    public class BatchQuery : IQuery
    {
        public IEnumerable<IQuery> Statements { get;  }
        
        /// <summary>
        /// Builds a new <see cref="BatchQuery"/> instance.
        /// </summary>
        /// <param name="queries">queries of the batch.</param>
        public BatchQuery(params IQuery[] queries)
        {
            Statements = queries;
        }
    }
}
