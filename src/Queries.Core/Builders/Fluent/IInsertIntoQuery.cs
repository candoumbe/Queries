using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public interface IInsertIntoQuery<T>
    {
        IBuildableQuery<T> Values(SelectQuery select);

        IBuildableQuery<T> Values(params InsertedValue[] values);

        


    }
}
