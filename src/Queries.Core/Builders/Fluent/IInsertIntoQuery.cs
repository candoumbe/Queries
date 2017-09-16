﻿using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders.Fluent
{
    public interface IInsertIntoQuery<T>
    {
        IBuild<T> Values(SelectQuery select);

        IBuild<T> Values(params InsertedValue[] values);

        


    }
}
