using System;
using Queries.Core.Builders.Fluent;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Query which result in creating a View.
    /// </summary>
    public class CreateViewQuery : IDataDefinitionQuery, IBuildableQuery<CreateViewQuery>
    {
        /// <summary>
        /// Name of the view
        /// </summary>
        public string ViewName { get;}
        
        /// <summary>
        /// <see cref="SelectQuery"/> the view will be built from
        /// </summary>
        public SelectQuery SelectQuery { get; private set; }

        /// <summary>
        /// Builds a new <see cref="CreateViewQuery"/> instance.
        /// </summary>
        /// <param name="viewName">Name of the view</param>
        internal CreateViewQuery(string viewName)
        {
            ViewName = viewName ?? throw new ArgumentNullException(nameof(viewName));
        }

        public IBuildableQuery<CreateViewQuery> As(SelectQuery select)
        {
            SelectQuery = select ?? throw new ArgumentNullException(nameof(select));

            return this;
        }


        public CreateViewQuery Build() => this;
    }
}
