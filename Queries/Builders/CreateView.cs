using System;
using Queries.Builders.Fluent;

namespace Queries.Builders
{
    public class CreateView : IBuildableQuery<CreateViewQuery>
    {
        private readonly CreateViewQuery _query;


        public CreateView(string viewName)
        {

            _query = new CreateViewQuery() { Name = viewName};
        }

        public IBuildableQuery<CreateViewQuery> As(SelectQuery select)
        {
            if (select == null)
            {
                throw new ArgumentNullException(nameof(select));
            }

            _query.As = select;

            return this;
        }

        public CreateViewQuery Build() => _query;

        
    }


}