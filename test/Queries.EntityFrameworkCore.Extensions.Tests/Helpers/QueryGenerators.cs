using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.EntityFrameworkCore.Extensions.Tests.Helpers
{
    public static class QueryGenerators
    {
        public static Arbitrary<CreateViewQuery> CreateViewGenerators()
        {
            Gen<NonWhiteSpaceString> viewNameGenerator = Arb.Generate<NonWhiteSpaceString>();

            Gen<IList<FieldColumn>> columnsGenerator = Arb.Default.NonWhiteSpaceString()
                                             .Generator
                                             .Select(column => column.Item.Field())
                                             .NonEmptyListOf();

            return Gen.Zip(viewNameGenerator, columnsGenerator).Select(tuple =>
            {
                (NonWhiteSpaceString view, IEnumerable<IColumn> cols) = tuple;
                return new CreateViewQuery(view.Item).As(Select(cols.ToArray()))
                                                     .Build();
            }).ToArbitrary();
        }
    }
}