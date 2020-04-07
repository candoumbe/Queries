using FluentAssertions;
using FluentAssertions.Extensions;
using NaughtyStrings;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using Queries.Core.Renderers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Columns.SelectColumn;

namespace Queries.Renderers.SqlServer.Tests
{
    [UnitTest]
    public class SqlServerRendererTest : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public SqlServerRendererTest(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void DefaultConstructor()
        {
            // Act
            SqlServerRenderer renderer = new SqlServerRenderer();

            // Assert
            renderer.Settings.Should().NotBeNull();
            renderer.Settings.PrettyPrint.Should().BeTrue($"{nameof(SqlServerRenderer)}.{nameof(SqlServerRenderer.Settings)}.{nameof(SqlServerRendererSettings.PrettyPrint)} should be set to true by default");
            renderer.Settings.DateFormatString.Should().Be("yyyy-MM-dd");
        }

        public static IEnumerable<object[]> SelectTestCases
        {
            get
            {
                yield return new object[] { Select(UUID()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT NEWID()" };

                yield return new object[] { Select(12.July(2010).Literal()), new SqlServerRendererSettings { PrettyPrint = false, DateFormatString = "dd/MM/yyyy" }, $"SELECT '{12.July(2010).ToString("dd/MM/yyyy")}'" };

                yield return new object[] { Select(1.Literal()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1" };

                yield return new object[] { Select(1L.Literal()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1 UNION SELECT 2" };

                yield return new object[] { Select(1.Literal()).Union(Select(2.Literal())), new SqlServerRendererSettings { PrettyPrint = true }, $"SELECT 1 {Environment.NewLine}UNION {Environment.NewLine}SELECT 2" };

                yield return new object[]
                {
                    Select("fullname")
                    .From(
                        Select(Concat("firstname".Field(), " ".Literal(),  "lastname".Field()).As("fullname"))
                        .From("people").As("p")
                    ),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [fullname] FROM (SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [people]) [p]"
                };

                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("people")
                        .Where("firstname".Field().IsNotNull()),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname], [lastname] FROM [people] WHERE ([firstname] IS NOT NULL)"
                };

                yield return new object[]
                {
                    Select("firstname".Field(), "lastname".Field())
                        .From("SuperHero")
                        .Where("Capabilities".Field().NotIn("Super strength", "Heat vision")),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'Super strength';" +
                    "DECLARE @p1 AS VARCHAR(8000) = 'Heat vision';" +
                    "SELECT [firstname], [lastname] FROM [SuperHero] WHERE ([Capabilities] NOT IN (@p0, @p1))"
                };

                yield return new object[]
                {
                    Select(1.2f.Literal()),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    $"SELECT {1.2f}"
                };

                yield return new object[]
                {
                    Select(double.MaxValue.Literal()),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    $"SELECT {double.MaxValue}"
                };

                yield return new object[]
                {
                    Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT * FROM (SELECT 1 UNION SELECT 2)"
                };

                yield return new object[] {
                    Select("*")
                    .From(
                        Select("identifier").From("identities").Union(Select("username").From("members")).As("logins")
                        ),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT * FROM (SELECT [identifier] FROM [identities] UNION SELECT [username] FROM [members]) [logins]"
                };

                yield return new object[]
                {
                    Select("*").From("Table"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT * FROM [Table]"
                };

                yield return new object[] {
                    Select("*".Field()).From("Table"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT * FROM [Table]" };

                yield return new object[] {
                    Select("Employees.*").From("Table"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [Employees].* FROM [Table]" };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new OrderExpression("firstname"))
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname]"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy(new OrderExpression("firstname", OrderDirection.Descending))
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT LEN([firstname] + ' ' + [lastname]) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Length(Min(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT LEN(MIN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Min(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                    .From("members")
                    .OrderBy("firstname".Desc())
                    , new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT MIN(LEN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
                };

                yield return new object[]
                {
                    Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Null("firstname".Field(), "").As("firstname")).From("members"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "SELECT ISNULL([firstname], '') AS [firstname] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Max("age".Field()).As("age maxi")).From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT MAX([age]) AS [age maxi] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT MAX(ISNULL([age], 0)) AS [age maxi] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Min("age".Field()).As("age mini")).From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT MIN([age]) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT MIN(ISNULL([age], 0)) AS [age mini] FROM [members]"
                };

                yield return new object[]
                {
                    Select("firstname".Field(), Max("age".Field()).As("age maximum"))
                    .From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT [firstname], MAX([age]) AS [age maximum] FROM [members] GROUP BY [firstname]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                    .From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0, 1) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Substring(Concat("firstname".Field(), "lastname".Field()), 0).As("initials"))
                    .From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname] + [lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                    .From("members"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
                };

                yield return new object[]
                {
                    Select(Upper("firstname".Field())),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT UPPER([firstname])"
                };

                yield return new object[]
                {
                    Select("Firstname".Field(), "Lastname".Field())
                    .From("SuperHeroes")
                    .Where("Nickname".Field(), EqualTo, "Batman"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'Batman';" +
                    "SELECT [Firstname], [Lastname] FROM [SuperHeroes] WHERE ([Nickname] = @p0)"
                };

                yield return new object[]
                {
                    Select("Firstname".Field(), "Lastname".Field())
                    .From("SuperHeroes")
                    .Where("Nickname".Field(), EqualTo, "Batman"),
                    new SqlServerRendererSettings{ PrettyPrint = true },
                    $"DECLARE @p0 AS VARCHAR(8000) = 'Batman';{Environment.NewLine}" +
                    $"SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                    $"FROM [SuperHeroes] {Environment.NewLine}" +
                    "WHERE ([Nickname] = @p0)"
                };

                yield return new object[]
                {
                    Select("*")
                    .From(
                        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                        .Union(
                        Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                    ),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "DECLARE @p0 AS VARCHAR(8000) = 'B%';" +
                    "SELECT * " +
                    "FROM (" +
                        "SELECT [Fullname] FROM [People] WHERE ([Firstname] LIKE @p0) " +
                        "UNION " +
                        "SELECT [Fullname] FROM [SuperHero] WHERE ([Nickname] LIKE @p0)" +
                    ")"
                };

                yield return new object[]
                {
                    Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                    new SqlServerRendererSettings(),
                    "DECLARE @p0 AS VARCHAR(8000) = 'Bruce';" +
                    "DECLARE @p1 AS VARCHAR(8000) = 'Bane';" +
                    "SELECT * FROM [members] WHERE ([Firstname] IN (@p0, @p1))"
                };

                yield return new object[]
                {
                    Select(
                        "Firstname".Field(),
                        "Lastname".Field(),
                        Cases(
                            When("Age".Field().GreaterThan(18), then : true),
                            When("Age".Field().IsNull(), then : false)
                        ).As("IsMajor"))
                        .From("members"),
                    new SqlServerRendererSettings(){ PrettyPrint = false },

                    "DECLARE @p0 AS NUMERIC = 18;" +
                    "DECLARE @p1 AS BIT = 1;" +
                    "DECLARE @p2 AS BIT = 0;" +
                    "SELECT [Firstname], [Lastname], CASE WHEN ([Age] > @p0) THEN @p1 WHEN ([Age] IS NULL) THEN @p2 AS [IsMajor] " +
                    "FROM [members]"
                };

                yield return new object[]
                {
                    Select(
                        "Firstname".Field(),
                        "Lastname".Field(),
                        Cases(
                            When("Age".Field().GreaterThan(18), then : true),
                            When("Age".Field().IsNull(), then : false)
                        ).As("IsMajor"))
                        .From("members"),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "DECLARE @p0 AS NUMERIC = 18;" +
                    "DECLARE @p1 AS BIT = 1;" +
                    "DECLARE @p2 AS BIT = 0;" +
                    "SELECT [Firstname], [Lastname], CASE WHEN ([Age] > @p0) THEN @p1 WHEN ([Age] IS NULL) THEN @p2 AS [IsMajor] " +
                    "FROM [members]"
                };

                yield return new object[]
                {
                    Select(Cases(
                        When("left".Field().Substract("right".Field()).LessThan(10), then : 1))
                        .Else(0)
                    )
                    .From("table1".Table("t1"))
                    .InnerJoin("table2".Table("t2"), "t1.Id".Field().EqualTo("t2.Id".Field())),
                    new SqlServerRendererSettings { PrettyPrint = false },
                    "DECLARE @p0 AS NUMERIC = 10;" +
                    "DECLARE @p1 AS NUMERIC = 1;" +
                    "DECLARE @p2 AS NUMERIC = 0;" +
                    "SELECT CASE WHEN ([left] - [right] < @p0) THEN @p1 ELSE @p2 " +
                    "FROM [table1] [t1] INNER JOIN [table2] [t2] " +
                    "ON ([t1].[Id] = [t2].[Id])"
                };
            }
        }

        public static IEnumerable<object[]> PaginateCases
        {
            get
            {
                yield return new object[]
                {
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: 1, pageSize:10),
                    new SqlServerRendererSettings(),
                    "SELECT TOP 10 [col1] FROM [table]"
                };
                {
                    var pagination = ( pageIndex: 2, pageSize: 10 );
                    yield return new object[]
                    {
                        Select("col1")
                            .From("table")
                            .Paginate(pageIndex: pagination.pageIndex, pageSize: pagination.pageSize),
                        new SqlServerRendererSettings(),
                        $"SELECT [col1] FROM [table] OFFSET {pagination.pageSize} ROWS " +
                        $"FETCH NEXT {pagination.pageSize} ROWS ONLY"
                    };
                }

                {
                    var pagination = (pageIndex: 3, pageSize: 10);
                    yield return new object[]
                    {
                        Select("col1")
                            .From("table")
                            .Paginate(pageIndex: pagination.pageIndex, pageSize: pagination.pageSize),
                        new SqlServerRendererSettings(),
                        $"SELECT [col1] FROM [table] OFFSET {pagination.pageSize} * ({pagination.pageIndex} - 1) ROWS " +
                        $"FETCH NEXT {pagination.pageSize} ROWS ONLY"
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(PaginateCases))]
        public void PaginateTest(SelectQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        [Theory]
        [MemberData(nameof(SelectTestCases))]
        public void SelectTest(SelectQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> FieldnameCasingStrategyCases
        {
            get
            {
                yield return new object[]
                {
                    Select("FirstName".Field(), "LastName".Field())
                        .From("members"),
                    FieldnameCasingStrategy.Default,
                    @"SELECT [FirstName], [LastName] FROM [members]"
                };

                yield return new object[]
                {
                    Select("FirstName".Field(), "LastName".Field())
                        .From("members"),
                    FieldnameCasingStrategy.CamelCase,
                    @"SELECT [firstName], [lastName] FROM [members]"
                };

                yield return new object[]
                {
                    Select("FirstName".Field(), "LastName".Field())
                        .From("members"),
                    FieldnameCasingStrategy.SnakeCase,
                    @"SELECT [first_name], [last_name] FROM [members]"
                };

                yield return new object[]
                {
                    Select("FirstName".Field(), "LastName".Field())
                        .From("members")
                        .Where(new WhereClause(Length(Null("MiddleName".Field(), string.Empty)), EqualTo, 0)),
                    FieldnameCasingStrategy.SnakeCase,
                    "DECLARE @p0 AS NUMERIC = 0;" +
                    "SELECT [first_name], [last_name] FROM [members] WHERE (LEN(ISNULL([middle_name], '')) = @p0)"
                };
            }
        }

        [Theory]
        [MemberData(nameof(FieldnameCasingStrategyCases))]
        public void CasingStrategy(SelectQuery query, FieldnameCasingStrategy casingStrategy, string expected)
        {
            // Arrange
            SqlServerRendererSettings settings = new SqlServerRendererSettings
            {
                FieldnameCasingStrategy = casingStrategy
            };

            SqlServerRenderer renderer = new SqlServerRenderer(settings);

            // Act
            string statement = renderer.Render(query);

            // Assert
            statement.Should()
                .Be(expected);
        }

        public static IEnumerable<object[]> CompileCases
        {
            get
            {
                yield return new object[]
                {
                    Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                    new SqlServerRendererSettings{ SkipVariableDeclaration = true },
                    (Expression<Func<CompiledQuery, bool>>)(
                        query => query.Statement == "SELECT * FROM [members] WHERE ([Firstname] IN (@p0, @p1))"
                            && query.Variables.Exactly(2)
                            && query.Variables.Once(v => v.Name == "p0" && "Bruce".Equals(v.Value) && v.Type == VariableType.String)
                            && query.Variables.Once(v => v.Name == "p1" && "Bane".Equals(v.Value) && v.Type == VariableType.String)
                    ),
                    "the statement contains 2 variables with 2 values"
                };

                yield return new object[]
                {
                    Select("*")
                    .From(
                        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                        .Union(
                        Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                    ),
                    new SqlServerRendererSettings{ PrettyPrint = false, SkipVariableDeclaration = true },
                    (Expression<Func<CompiledQuery, bool>>)(
                        query => query.Statement == "SELECT * FROM (" +
                            "SELECT [Fullname] FROM [People] WHERE ([Firstname] LIKE @p0) " +
                            "UNION " +
                            "SELECT [Fullname] FROM [SuperHero] WHERE ([Nickname] LIKE @p0)" +
                        ")"
                            && query.Variables.Once()
                            && query.Variables.Once(v => v.Name == "p0" && "B%".Equals(v.Value) && v.Type == VariableType.String)
                    ),
                    "The select statement as two variables with SAME value"
                };

                //yield return new object[]
                //{
                //    Select("*")
                //    .From(
                //        Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                //        .Union(
                //        Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                //    ),
                //    new SqlServerRendererSettings{ PrettyPrint = true, SkipVariableDeclaration = true },
                //    (Expression<Func<CompiledQuery, bool>>)(
                //        query => query.Statement == $"SELECT * {Environment.NewLine}FROM ("
                //                                    + $"SELECT [Fullname] {Environment.NewLine}FROM [People] {Environment.NewLine}WHERE ([Firstname] LIKE @p0) "
                //                                    + $"UNION {Environment.NewLine}"
                //                                    + $"SELECT [Fullname] {Environment.NewLine}FROM [SuperHero] {Environment.NewLine}WHERE ([Nickname] LIKE @p0)"
                //                                    + ")"
                //            && query.Variables.Once()
                //            && query.Variables.Once(v => v.Name == "p0" && "B%".Equals(v.Value) && v.Type == VariableType.String)
                //    ),
                //    "The select statement as two variables with SAME value"
                //};
            }
        }

        [Theory]
        [MemberData(nameof(CompileCases))]
        public void Select_Compile(SelectQuery query, SqlServerRendererSettings settings, Expression<Func<CompiledQuery, bool>> expectation, string reason)
        {
            // Arrange
            SqlServerRenderer renderer = new SqlServerRenderer(settings);

            // Assert
            CompiledQuery compiledQuery = renderer.Compile(query);

            _outputHelper.WriteLine($"{nameof(compiledQuery)} : '{compiledQuery}'");
            _outputHelper.WriteLine($"{nameof(CompiledQuery)}.{nameof(CompiledQuery.Statement)} : '{compiledQuery.Statement}'");

            // Assert
            compiledQuery.Should()
                .Match(expectation, reason);
        }

        public static IEnumerable<object[]> UpdateTestCases
        {
            get
            {
                yield return new object[]
                {
                    Update("members").Set("UUID".Field().UpdateValueTo(UUID())),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [UUID] = NEWID()"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().UpdateValueTo("")).Where("firstname".Field().IsNull()),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [firstname] = '' WHERE ([firstname] IS NULL)"
                };
                yield return new object[]
                {
                    Update("members").Set("firstname".Field().UpdateValueTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "UPDATE [members] SET [firstname] = NULL WHERE ([firstname] = '')"
                };
            }
        }

        [Theory]
        [MemberData(nameof(UpdateTestCases))]
        public void UpdateTest(UpdateQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> BatchQueryCases
        {
            get
            {
                yield return new object[]
                {
                    new BatchQuery(
                        Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                        Select("*").From("members")
                    ),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "DELETE FROM [members] WHERE ([firstname] IS NULL);" +
                    "SELECT * FROM [members];"
                };
            }
        }

        [Theory]
        [MemberData(nameof(BatchQueryCases))]
        public void BatchQueryTest(BatchQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> TruncateQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "TRUNCATE TABLE [SuperHero]"
                };
                yield return new object[]
                {
                    Truncate("SuperHero"),
                    new SqlServerRendererSettings{ PrettyPrint = true },
                    "TRUNCATE TABLE [SuperHero]"
                };
            }
        }

        [Theory]
        [MemberData(nameof(TruncateQueryCases))]
        public void TruncateQueryTest(TruncateQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> DeleteQueryCases
        {
            get
            {
                yield return new object[]
                {
                    Delete("members")
                    .Where("Activity".Field(), NotLike, "%Super hero%"),
                    new SqlServerRendererSettings(),
                    "DECLARE @p0 AS VARCHAR(8000) = '%Super hero%';" +
                    "DELETE FROM [members] WHERE ([Activity] NOT LIKE @p0)"
                };
            }
        }

        [Theory]
        [MemberData(nameof(DeleteQueryCases))]
        public void DeleteQueryTests(DeleteQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> SelectIntoQueryCases
        {
            get
            {
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    new SqlServerRendererSettings{ PrettyPrint = false },
                    "SELECT * INTO [SuperHero_BackUp] FROM (SELECT [Firstname], [Lastname] FROM [DCComics])"
                };
                yield return new object[]
                {
                    SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                    new SqlServerRendererSettings{ PrettyPrint = true },
                    $"SELECT * {Environment.NewLine}" +
                    $"INTO [SuperHero_BackUp] {Environment.NewLine}" +
                    $"FROM (SELECT [Firstname], [Lastname] {Environment.NewLine}" +
                    $"FROM [DCComics])"
                };
            }
        }

        [Theory]
        [MemberData(nameof(SelectIntoQueryCases))]
        public void SelectIntoQueryTest(SelectIntoQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> SqlInjectionAttackCases
        {
            get
            {
                yield return new object[]
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), EqualTo, "Dupont';--"),
                    new SqlServerRendererSettings(),

                    "DECLARE @p0 AS VARCHAR(8000) = 'Dupont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] = @p0)"
                };

                yield return new object[]
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), Like, "Du[pont';--"),
                    new SqlServerRendererSettings(),

                    @"DECLARE @p0 AS VARCHAR(8000) = 'Du\[pont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] LIKE @p0)"
                };

#if !NETCOREAPP1_0
                {
                    foreach (string naughtyString in TheNaughtyStrings.SQLInjection)
                    {
                        string escapedString = naughtyString
                            .Replace("\'", "''")
                            .Replace("[", "[");
                        yield return new object[]
                        {
                            Select("*").From("superheroes")
                                .Where("name".Field(), EqualTo, naughtyString),
                            new SqlServerRendererSettings (),
                            $"DECLARE @p0 AS VARCHAR(8000) = '{escapedString}';" +
                            "SELECT * FROM [superheroes] WHERE ([name] = @p0)"
                        };
                    }
                }
#endif
            }
        }

        [Theory]
        [MemberData(nameof(SqlInjectionAttackCases))]
        public void PreventSqlInjectionAttack(IQuery query, SqlServerRendererSettings settings, string expectedString) => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> CreateViewCases
        {
            get
            {
                yield return new object[]
                {
                    CreateView("active_users")
                    .As(Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field() ))
                        .From("members")
                        .Where("IsActive".Field().EqualTo(true))
                        .Build()),
                    new SqlServerRendererSettings (),
                    "CREATE VIEW [active_users] " +
                    "AS SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE ([IsActive] = 1)"
                };
            }
        }

        [Theory]
        [MemberData(nameof(CreateViewCases))]
        public void CreateViewTest(CreateViewQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        public static IEnumerable<object[]> InsertIntoQueryCases
        {
            get
            {
                yield return new object[]
                {
                    InsertInto("members")
                    .Values(
                        "Firstname".Field().InsertValue("Bruce".Literal()),
                        "Lastname".Field().InsertValue("Wayne".Literal())
                    ),
                    new SqlServerRendererSettings(),
                    "INSERT INTO [members] ([Firstname], [Lastname]) VALUES ('Bruce', 'Wayne')"
                };
            }
        }

        [Theory]
        [MemberData(nameof(InsertIntoQueryCases))]
        public void InsertIntoQueryTest(InsertIntoQuery query, SqlServerRendererSettings settings, string expectedString)
            => IsQueryOk(query, settings, expectedString);

        private void IsQueryOk(IQuery query, SqlServerRendererSettings settings, string expectedString)
        {
            _outputHelper.WriteLine($"{nameof(query)} : {query}");
            _outputHelper.WriteLine($"{nameof(settings)} : {settings}");
            // Act
            string result = query.ForSqlServer(settings);

            // Assert
            result.Should().Be(expectedString);
        }
    }
}
