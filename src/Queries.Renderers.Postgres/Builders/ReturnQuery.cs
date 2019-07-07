using Optional;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries.Renderers.Postgres.Builders
{
    /// <summary>
    /// Wraps a return expression around <see cref="InsertIntoQuery"/> instance
    /// </summary>
    public class ReturnQuery : IQuery
    {
        /// <summary>
        /// Builds a new <see cref="ReturnQuery"/> instance that wraps a <see cref="InsertIntoQuery"/>.
        /// </summary>
        /// <param name="return">The value/expression to return</param>
        /// 
        public ReturnQuery(Option<ColumnBase, SelectQuery> @return) => Return = @return;

        public Option<ColumnBase, SelectQuery> Return { get; }
    }
}
