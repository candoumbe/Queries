using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Represents many <see cref="IQuery"/>s that will be executed as a whole
    /// </summary>
    public class BatchQuery : IQuery
    {
        private readonly List<IQuery> _statements;

        /// <summary>
        /// Statements that the current instance holds.
        /// </summary>
        public IEnumerable<IQuery> Statements => _statements;

        /// <summary>
        /// Builds a new <see cref="BatchQuery"/> instance.
        /// </summary>
        /// <param name="queries">queries of the batch.</param>
        public BatchQuery(params IQuery[] queries) => _statements = queries.Where(x => x is not null)
                                                                           .ToList();

        /// <summary>
        /// Adds a statement to this instance
        /// </summary>
        /// <param name="query">The query to add</param>
        /// <returns></returns>
        public BatchQuery AddStatement(IQuery query)
        {
            _statements.Add(query);

            return this;
        }

        /// <summary>
        /// Adds multiple statements to the current instance
        /// </summary>
        /// <param name="queries"></param>
        /// <returns>The current instance</returns>
        public BatchQuery AddStatements(IEnumerable<IQuery> queries)
        {
            _statements.AddRange(queries.Where(q => q is not null));

            return this;
        }
    }
}
