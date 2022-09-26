using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FsCheck;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace TestsHelpers;

/// <summary>
/// Generators used for property based testing
/// </summary>
[ExcludeFromCodeCoverage]
public static class QueryGenerators
{
    /// <summary>
    /// Builds generators for <see cref="CreateViewQuery"/>
    /// </summary>
    public static Arbitrary<CreateViewQuery> CreateViewGenerators()
    {
        Gen<NonWhiteSpaceString> viewNameGenerator = Arb.Generate<NonWhiteSpaceString>();

        Gen<IList<FieldColumn>> columnsGenerator = FieldColumnGenerators().Generator
                                                                          .NonEmptyListOf();

        return Gen.Zip(viewNameGenerator, columnsGenerator).Select(tuple =>
        {
            (NonWhiteSpaceString view, IEnumerable<IColumn> cols) = tuple;
            return new CreateViewQuery(view.Item).As(Select(cols.ToArray()))
                                                 .Build();
        }).ToArbitrary();
    }

    /// <summary>
    /// Builds generators for <see cref="FieldColumn"/>
    /// </summary>
    public static Arbitrary<FieldColumn> FieldColumnGenerators()
    {
        return Arb.Generate<NonWhiteSpaceString>()
                   .Select(columnName => columnName.Item.Field())
                   .ToArbitrary();
    }

    /// <summary>
    /// Builds generators for <see cref="DeleteQuery"/>
    /// </summary>
    public static Arbitrary<DeleteQuery> DeleteQueryGenerators()
    {
        Gen<NonWhiteSpaceString> tableNameGenerator = Arb.Generate<NonWhiteSpaceString>();

        Gen<IList<FieldColumn>> columnsGenerator = FieldColumnGenerators().Generator
                                                                          .NonEmptyListOf();

        Gen<IColumn> constraintGenerator = FieldColumnGenerators().Generator
                                                                  .Select(field => field.As<IColumn>());

        return Gen.Zip(tableNameGenerator, columnsGenerator, constraintGenerator)
                  .Select(tuple =>
                    {
                        (NonWhiteSpaceString tableName, IEnumerable<FieldColumn> cols, IColumn constraint) = tuple;
                        IWhereClause where = null;

                        return new DeleteQuery(tableName.Item)
                                    .Where(where)
                                    .Build();
                    }).ToArbitrary();
    }

    private static Arbitrary<ClauseOperator> ClauseOperatorGenerators()
        => Gen.Elements(Enum.GetValues(typeof(ClauseOperator)).Cast<ClauseOperator>()).ToArbitrary();

    private static Arbitrary<WhereClause> WhereGenerator(Gen<IColumn> columnGenerator)
    {
        Gen<ClauseOperator> opGenerator = ClauseOperatorGenerators().Generator;
        Gen<IColumn> valueGenerator = FieldColumnGenerators().Generator.Select(column => column as IColumn);

        return Gen.Zip(columnGenerator, opGenerator, valueGenerator)
                  .Select(tuple =>
                  {
                      (IColumn columnName, ClauseOperator op, IColumn constraint) = tuple;

                      return new WhereClause(columnName, op, constraint);
                  }).ToArbitrary();
    }
}