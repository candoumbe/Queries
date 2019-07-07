using Optional;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queries.Renderers.Postgres.Builders.Fluent
{
    /// <summary>
    /// Fluent builder for <see cref="ReturnQuery"/>
    /// </summary>
    public static class ReturnBuilder
    {
        /// <summary>
        /// Builds a <see cref="ReturnQuery"/> which returns <paramref name="returnValue"/> for <paramref name="query"/>.
        /// </summary>
        /// <param name="returnValue">The "value" to return</param>
        /// 
        /// <returns></returns>
        public static ReturnQuery Return(ColumnBase returnValue)
        {
            if (returnValue == null)
            {
                throw new ArgumentNullException(nameof(returnValue));
            }
            return new ReturnQuery(Option.Some<ColumnBase, SelectQuery>(returnValue));
        }

        /// <summary>
        /// Builds a <see cref="ReturnQuery"/> which returns <paramref name="returnValue"/> for <paramref name="query"/>.
        /// </summary>
        /// <param name="returnValue">The "value" to return</param>
        /// 
        /// <returns></returns>
        public static ReturnQuery Return(SelectQuery returnValue)
        {
            if (returnValue == null)
            {
                throw new ArgumentNullException(nameof(returnValue));
            }
            return new ReturnQuery(Option.None<ColumnBase, SelectQuery>(returnValue));
        }

        /// <summary>
        /// Builds a <see cref="ReturnQuery"/> which returns.
        /// </summary>
        /// <param name="returnValue">The "value" to return</param
        /// <returns></returns>
        public static ReturnQuery Return() => new ReturnQuery(Option.None<ColumnBase, SelectQuery>(default));
    }
}
