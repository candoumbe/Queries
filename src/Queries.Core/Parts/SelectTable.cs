using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts
{
    /// <summary>
    /// Wrapper  that turns a <see cref="SelectQuery"/> into a <see cref="ITable"/>.
    /// </summary>
    /// <remarks>
    /// This class allows to use a <see cref="SelectQuery"/> into  <see cref="Builders.Fluent.IFromQuery{T}"/>.
    /// </remarks>
    public class SelectTable : IAliasable<SelectTable>, ITable
    {
        private string _alias;
        /// <summary>
        /// The original <see cref="SelectQuery"/>
        /// </summary>
        public SelectQuery Select { get;}

        public string Alias => _alias;

        public SelectTable As(string alias)
        {
            _alias = alias;
            return this;
        }

        public ITable Clone() => Select.Clone();

        /// <summary>
        /// Builds a new <see cref="SelectTable"/> instance.
        /// </summary>
        /// <param name="query">the select query to wrap as an <see cref="ITable"/></param>
        public SelectTable(SelectQuery query)
        {
            Select = query ?? throw new ArgumentNullException(nameof(query));
        }
    }
}