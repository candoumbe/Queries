
using Queries.Core.Parts.Clauses;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Renderers
{
    /// <summary>
    /// The extension point to implement to provide conversion from <see cref="IQuery"/> to its a database equivalent <see cref="string"/>
    /// </summary>
    public interface IQueryRenderer
    {
        /// <summary>
        /// Converts the specified <see cref="IQuery"/> to its provider
        /// </summary>
        /// <param name="query">The query to render</param>
        /// <returns>The specific string</returns>
        string Render(IQuery query);

#if NETSTANDARD2_1
        public (string query, IEnumerable<Variable> parameters) Explain(IQuery query)
        {
            string queryString = Render(query);

            return (queryString, parameters: Enumerable.Empty<Variable>());
        }
#endif
    }
}

