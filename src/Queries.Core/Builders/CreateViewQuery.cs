using System;
using Queries.Core.Builders.Fluent;

namespace Queries.Core.Builders
{
    public class CreateViewQuery : IDataDefinitionQuery, IBuildableQuery<CreateViewQuery>
    {
        public string ViewName { get; private set; }
        
        public SelectQuery SelectQuery { get; private set; }

        internal CreateViewQuery(string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }
            ViewName = viewName;
        }

        public IBuildableQuery<CreateViewQuery> As(SelectQuery select)
        {
            if (select == null)
            {
                throw new ArgumentNullException(nameof(select));
            }

            SelectQuery = select;

            return this;
        }


        public CreateViewQuery Build() => this;
    }
}
