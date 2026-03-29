using System;
using System.Collections.Generic;
using AwesomeAssertions;
using AwesomeAssertions.Extensions;
using NaughtyStrings;
using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Builders.Fluent;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Sorting;
using Queries.Core.Renderers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;
using static Queries.Core.Parts.Columns.SelectColumn;

namespace Queries.Renderers.SqlServer.Tests;

[UnitTest]
[Feature("Sql Server")]
public class SqlServerRendererTest(ITestOutputHelper outputHelper)
{
    [Fact]
    public void DefaultConstructor()
    {
        // Act
        SqlServerRenderer renderer = new();

        // Assert
        renderer.Settings.Should().NotBeNull();
        renderer.Settings.PrettyPrint.Should().BeTrue($"{nameof(SqlServerRenderer)}.{nameof(SqlServerRenderer.Settings)}.{nameof(SqlServerRendererSettings.PrettyPrint)} should be set to true by default");
        renderer.Settings.DateFormatString.Should().Be("yyyy-MM-dd");
        renderer.Settings.Parametrization.Should().Be(ParametrizationSettings.Default);
    }


    public static TheoryData<IPaginatedQuery<SelectQuery>, SqlServerRendererSettings, string> PaginateCases
    {
        get
        {
            TheoryData<IPaginatedQuery<SelectQuery>, SqlServerRendererSettings, string> cases = new();
            cases.Add(
                Select("col1")
                    .From("table")
                    .Paginate(pageIndex: 1, pageSize:10),
                new SqlServerRendererSettings(),
                "SELECT TOP 10 [col1] FROM [table]"
            );
            {
                (int pageIndex, int pageSize) pagination = (pageIndex: 2, pageSize: 10);
                cases.Add(
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: pagination.pageIndex, pageSize: pagination.pageSize),
                    new SqlServerRendererSettings(),
                    $"SELECT [col1] FROM [table] OFFSET {pagination.pageSize} ROWS " +
                    $"FETCH NEXT {pagination.pageSize} ROWS ONLY"
                );
            }
            {
                (int pageIndex, int pageSize) pagination = (pageIndex: 3, pageSize: 10);
                cases.Add(
                    Select("col1")
                        .From("table")
                        .Paginate(pageIndex: pagination.pageIndex, pageSize: pagination.pageSize),
                    new SqlServerRendererSettings(),
                    $"SELECT [col1] FROM [table] OFFSET {pagination.pageSize} * ({pagination.pageIndex} - 1) ROWS " +
                    $"FETCH NEXT {pagination.pageSize} ROWS ONLY"
                );
            }

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(PaginateCases))]
    public void PaginateTest(SelectQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<SelectQuery>, SqlServerRendererSettings, string> SelectTestCases
        => new()
        {
             { Select(UUID()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT NEWID()" },

            { Select(12.July(2010).Literal()), new SqlServerRendererSettings { PrettyPrint = false, DateFormatString = "dd/MM/yyyy" }, $"SELECT '{12.July(2010).ToString("dd/MM/yyyy")}'" },

            { Select(1.Literal()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1" },

            { Select(1L.Literal()), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1" },

            { Select(1.Literal()).Union(Select(2.Literal())), new SqlServerRendererSettings { PrettyPrint = false }, "SELECT 1 UNION SELECT 2" },

            { Select(1.Literal()).Union(Select(2.Literal())), new SqlServerRendererSettings { PrettyPrint = true }, $"SELECT 1{Environment.NewLine}UNION{Environment.NewLine}SELECT 2" },


            {
                Select("fullname")
                .From(
                    Select(Concat("firstname".Field(), " ".Literal(),  "lastname".Field()).As("fullname"))
                    .From("people").As("p")
                ),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [fullname] FROM (SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [people]) [p]"
            },
            {
                Select("firstname".Field(), "lastname".Field())
                    .From("people")
                    .Where("firstname".Field().IsNotNull()),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname], [lastname] FROM [people] WHERE ([firstname] IS NOT NULL)"
            },
            {
                Select("firstname".Field(), "lastname".Field())
                    .From("SuperHero")
                    .Where("Capabilities".Field().NotIn("Super strength", "Heat vision")),
                new SqlServerRendererSettings { PrettyPrint = false },
                "DECLARE @p0 AS VARCHAR(8000) = 'Super strength';" +
                "DECLARE @p1 AS VARCHAR(8000) = 'Heat vision';" +
                "SELECT [firstname], [lastname] FROM [SuperHero] WHERE ([Capabilities] NOT IN (@p0, @p1))"
            },
            {
                Select(1.2f.Literal()),
                new SqlServerRendererSettings { PrettyPrint = false },
                $"SELECT {1.2f}"
            },
            {
                Select(double.MaxValue.Literal()),
                new SqlServerRendererSettings { PrettyPrint = false },
                $"SELECT {double.MaxValue}"
            },
            {
                Select("*").From(Select(1.Literal()).Union(Select(2.Literal()))),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT * FROM (SELECT 1 UNION SELECT 2)"
            },
            {
                Select("*")
                .From(
                    Select("identifier").From("identities").Union(Select("username").From("members")).As("logins")
                    ),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT * FROM (SELECT [identifier] FROM [identities] UNION SELECT [username] FROM [members]) [logins]"
            },
            {
                Select("*").From("Table"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT * FROM [Table]"
            },
            {
                Select("*".Field()).From("Table"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT * FROM [Table]"
            },
            {
                Select("Employees.*").From("Table"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [Employees].* FROM [Table]"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members]"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field())).From("members"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members]"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members]"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy(new OrderExpression("firstname"))
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname]"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy(new OrderExpression("firstname", OrderDirection.Descending))
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))
                .From("members")
                .OrderBy("firstname".Desc())
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] FROM [members] ORDER BY [firstname] DESC"
            },
            {
                Select(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field())))
                .From("members")
                .OrderBy("firstname".Desc())
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT LEN([firstname] + ' ' + [lastname]) FROM [members] ORDER BY [firstname] DESC"
            },
            {
                Select(Length(Min(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                .From("members")
                .OrderBy("firstname".Desc())
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT LEN(MIN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
            },
            {
                Select(Min(Length(Concat("firstname".Field(), " ".Literal(), "lastname".Field()))))
                .From("members")
                .OrderBy("firstname".Desc())
                , new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT MIN(LEN([firstname] + ' ' + [lastname])) FROM [members] ORDER BY [firstname] DESC"
            },
            {
                Select(Concat("firstname".Field(), " ".Literal(), "lastname".Field()).As("fullname")).From("members"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [members]"
            },
            {
                Select(Null("firstname".Field(), "").As("firstname")).From("members"),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT ISNULL([firstname], '') AS [firstname] FROM [members]"
            },
            {
                Select(Max("age".Field()).As("age maxi")).From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT MAX([age]) AS [age maxi] FROM [members]"
            },
            {
                Select(Max(Null("age".Field(), 0)).As("age maxi")).From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT MAX(ISNULL([age], 0)) AS [age maxi] FROM [members]"
            },
            {
                Select(Min("age".Field()).As("age mini")).From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT MIN([age]) AS [age mini] FROM [members]"
            },
            {
                Select(Min(Null("age".Field(), 0)).As("age mini")).From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT MIN(ISNULL([age], 0)) AS [age mini] FROM [members]"
            },
            {
                Select("firstname".Field(), Max("age".Field()).As("age maximum"))
                .From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT [firstname], MAX([age]) AS [age maximum] FROM [members] GROUP BY [firstname]"
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0, 1)).As("initials"))
                .From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0, 1) AS [initials] FROM [members]"
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                .From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
            },
            {
                Select(Substring(Concat("firstname".Field(), "lastname".Field()), 0).As("initials"))
                .From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT SUBSTRING([firstname] + [lastname], 0) AS [initials] FROM [members]"
            },
            {
                Select(Concat(Substring("firstname".Field(), 0, 1), Substring("lastname".Field(), 0)).As("initials"))
                .From("members"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT SUBSTRING([firstname], 0, 1) + SUBSTRING([lastname], 0) AS [initials] FROM [members]"
            },
            {
                Select(Upper("firstname".Field())),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT UPPER([firstname])"
            },
            {
                Select("Firstname".Field(), "Lastname".Field())
                .From("SuperHeroes")
                .Where("Nickname".Field(), EqualTo, "Batman"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "DECLARE @p0 AS VARCHAR(8000) = 'Batman';" +
                "SELECT [Firstname], [Lastname] FROM [SuperHeroes] WHERE ([Nickname] = @p0)"
            },
            {
                Select("Firstname".Field(), "Lastname".Field())
                .From("SuperHeroes")
                .Where("Nickname".Field(), EqualTo, "Batman"),
                new SqlServerRendererSettings{ PrettyPrint = true },
                $"DECLARE @p0 AS VARCHAR(8000) = 'Batman';{Environment.NewLine}" +
                $"SELECT{Environment.NewLine}" +
                $"    [Firstname], [Lastname]{Environment.NewLine}" +
                $"FROM{Environment.NewLine}" +
                $"    [SuperHeroes]{Environment.NewLine}" +
                "WHERE ([Nickname] = @p0)"
            },
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
            },
            {
                Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                new SqlServerRendererSettings {PrettyPrint = false },
                "DECLARE @p0 AS VARCHAR(8000) = 'Bruce';" +
                "DECLARE @p1 AS VARCHAR(8000) = 'Bane';" +
                "SELECT * FROM [members] WHERE ([Firstname] IN (@p0, @p1))"
            },
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
                "SELECT [Firstname], [Lastname], CASE WHEN ([Age] > @p0) THEN @p1 WHEN ([Age] IS NULL) THEN @p2 END AS [IsMajor] " +
                "FROM [members]"
            },
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
                "SELECT [Firstname], [Lastname], CASE WHEN ([Age] > @p0) THEN @p1 WHEN ([Age] IS NULL) THEN @p2 END AS [IsMajor] " +
                "FROM [members]"
            },
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
                "SELECT CASE WHEN ([left] - [right] < @p0) THEN @p1 ELSE @p2 END " +
                "FROM [table1] [t1] INNER JOIN [table2] [t2] " +
                "ON ([t1].[Id] = [t2].[Id])"
            },
            {
                Select("col1", "col2")
                .From("table")
                .Where("col1".Field().In("val1", "val2")),
                new SqlServerRendererSettings { PrettyPrint = false },
                "DECLARE @p0 AS VARCHAR(8000) = 'val1';" +
                "DECLARE @p1 AS VARCHAR(8000) = 'val2';" +
                "SELECT [col1], [col2] FROM [table] WHERE ([col1] IN (@p0, @p1))"
            },
            {
                Select("col1", "col2")
                .From("table")
                .Where("col1".Field().In("val1", "val2")),
                new SqlServerRendererSettings { PrettyPrint = false, Parametrization = ParametrizationSettings.None },
                "SELECT [col1], [col2] FROM [table] WHERE ([col1] IN ('val1', 'val2'))"
            },
            {
                Select("col1", "col2")
                .From("table")
                .Where("col1".Field().In(Select("col3").From("table2").Build())),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [col1], [col2] FROM [table] WHERE ([col1] IN (SELECT [col3] FROM [table2]))"
            },
            {
                Select("col1", "col2")
                .From("table")
                .Where("col1".Field().NotIn("val1", "val2")),
                new SqlServerRendererSettings { PrettyPrint = false, Parametrization = ParametrizationSettings.None },
                "SELECT [col1], [col2] FROM [table] WHERE ([col1] NOT IN ('val1', 'val2'))"
            },
            {
                Select("col1", "col2")
                .From("table")
                .Where("col1".Field().NotIn(Select("col3").From("table2").Build())),
                new SqlServerRendererSettings { PrettyPrint = false },
                "SELECT [col1], [col2] FROM [table] WHERE ([col1] NOT IN (SELECT [col3] FROM [table2]))"
            }
        };

    [Theory]
    [MemberData(nameof(SelectTestCases))]
    public void SelectTest(SelectQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<SelectQuery>, FieldnameCasingStrategy, string> FieldnameCasingStrategyCases
        => new ()
        {
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.Default,
                "SELECT [FirstName], [LastName] FROM [members]"
            },
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.CamelCase,
                "SELECT [firstName], [lastName] FROM [members]"
            },
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members"),
                FieldnameCasingStrategy.SnakeCase,
                "SELECT [first_name], [last_name] FROM [members]"
            },
            {
                Select("FirstName".Field(), "LastName".Field())
                    .From("members")
                    .Where(new WhereClause(Length(Null("MiddleName".Field(), string.Empty)), EqualTo, 0)),
                FieldnameCasingStrategy.SnakeCase,
                "DECLARE @p0 AS NUMERIC = 0;" +
                "SELECT [first_name], [last_name] FROM [members] WHERE (LEN(ISNULL([middle_name], '')) = @p0)"
            }
        };

    [Theory]
    [MemberData(nameof(FieldnameCasingStrategyCases))]
    public void CasingStrategy(SelectQuery query, FieldnameCasingStrategy casingStrategy, string expected)
    {
        // Arrange
        SqlServerRendererSettings settings = new()
        {
            FieldnameCasingStrategy = casingStrategy,
            PrettyPrint = false
        };

        SqlServerRenderer renderer = new(settings);

        // Act
        string statement = renderer.Render(query);

        // Assert
        statement.Should()
            .Be(expected);
    }

    public static TheoryData<IQuery, SqlServerRendererSettings, CompiledQuery, string> CompileCases
        => new()
        {
            {
                Select("*").From("members").Where("Firstname".Field(), In, new StringValues("Bruce", "Bane")),
                new SqlServerRendererSettings{ Parametrization = ParametrizationSettings.SkipVariableDeclaration },
                new (
                    "SELECT * FROM [members] WHERE ([Firstname] IN (@p0, @p1))",
                    [
                        new Variable("p0", VariableType.String, "Bruce"),
                        new Variable("p1", VariableType.String, "Bane")
                    ]
                ),
                "the statement contains 2 variables with 2 values"
            },
            {
                Select("*")
                .From(
                    Select("Fullname").From("People").Where("Firstname".Field(), Like, "B%")
                    .Union(
                    Select("Fullname").From("SuperHero").Where("Nickname".Field(), Like, "B%"))
                ),
                new SqlServerRendererSettings{ PrettyPrint = false, Parametrization = ParametrizationSettings.SkipVariableDeclaration },
                new (
                    "SELECT * FROM (" +
                        "SELECT [Fullname] FROM [People] WHERE ([Firstname] LIKE @p0) " +
                        "UNION " +
                        "SELECT [Fullname] FROM [SuperHero] WHERE ([Nickname] LIKE @p0)" +
                    ")",
                    [
                        new Variable("p0", VariableType.String, "B%")
                    ]
                ),
                "The select statement as two variables with SAME value"
            }
        };

    [Theory]
    [MemberData(nameof(CompileCases))]
    public void GivenCompile(IQuery query, SqlServerRendererSettings settings, CompiledQuery expected, string reason)
    {
        // Arrange
        SqlServerRenderer renderer = new(settings);

        // Assert
        CompiledQuery actual = renderer.Compile(query);

        outputHelper.WriteLine($"{nameof(actual)} : '{actual}'");
        outputHelper.WriteLine($"{nameof(CompiledQuery)}.{nameof(CompiledQuery.Statement)} : '{actual.Statement}'");

        // Assert
        actual.Should().Be(expected);
    }

    public static TheoryData<UpdateQuery, SqlServerRendererSettings, string> UpdateTestCases
        => new()
        {
            {
                Update("members").Set("UUID".Field().UpdateValueTo(UUID())),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "UPDATE [members] SET [UUID] = NEWID()"
            },
            {
                Update("members").Set("firstname".Field().UpdateValueTo("")).Where("firstname".Field().IsNull()),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "UPDATE [members] SET [firstname] = '' WHERE ([firstname] IS NULL)"
            },
            {
                Update("members").Set("firstname".Field().UpdateValueTo(null)).Where(new WhereClause("firstname".Field(), EqualTo, "")),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "UPDATE [members] SET [firstname] = NULL WHERE ([firstname] = '')"
            }
        };

    [Theory]
    [MemberData(nameof(UpdateTestCases))]
    public void UpdateTest(UpdateQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<BatchQuery, SqlServerRendererSettings, string> BatchQueryCases
        => new()
        {
            {
                new BatchQuery(
                    Delete("members").Where(new WhereClause("firstname".Field(), IsNull)),
                    Select("*").From("members")
                ),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "DELETE FROM [members] WHERE ([firstname] IS NULL);" +
                "SELECT * FROM [members];"
            }
        };

    [Theory]
    [MemberData(nameof(BatchQueryCases))]
    public void BatchQueryTest(BatchQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<TruncateQuery, SqlServerRendererSettings, string> TruncateQueryCases
        => new()
        {
            {
                Truncate("SuperHero"),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "TRUNCATE TABLE [SuperHero]"
            },
            {
                Truncate("SuperHero"),
                new SqlServerRendererSettings{ PrettyPrint = true },
                "TRUNCATE TABLE [SuperHero]"
            }
        };

    [Theory]
    [MemberData(nameof(TruncateQueryCases))]
    public void TruncateQueryTest(TruncateQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<DeleteQuery>, SqlServerRendererSettings, string> DeleteQueryCases
        => new()
        {
            {
                Delete("members")
                .Where("Activity".Field(), NotLike, "%Super hero%"),
                new SqlServerRendererSettings(),
                "DECLARE @p0 AS VARCHAR(8000) = '%Super hero%';" +
                "DELETE FROM [members] WHERE ([Activity] NOT LIKE @p0)"
            }
        };

    [Theory]
    [MemberData(nameof(DeleteQueryCases))]
    public void DeleteQueryTests(DeleteQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<SelectIntoQuery>, SqlServerRendererSettings, string> SelectIntoQueryCases
        => new()
        {
            {
                SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                new SqlServerRendererSettings{ PrettyPrint = false },
                "SELECT * INTO [SuperHero_BackUp] FROM (SELECT [Firstname], [Lastname] FROM [DCComics])"
            },
            {
                SelectInto("SuperHero_BackUp").From(Select("Firstname", "Lastname").From("DCComics")),
                new SqlServerRendererSettings{ PrettyPrint = true },
                $"SELECT{Environment.NewLine}" +
                $"    *{Environment.NewLine}" +
                $"INTO{Environment.NewLine}" +
                $"    [SuperHero_BackUp]{Environment.NewLine}" +
                $"FROM{Environment.NewLine}" +
                $"({Environment.NewLine}" +
                $"    SELECT{Environment.NewLine}" +
                $"        [Firstname], [Lastname]{Environment.NewLine}" +
                $"    FROM{Environment.NewLine}" +
                $"        [DCComics]{Environment.NewLine}" +
                 ")"
            }
        };

    [Theory]
    [MemberData(nameof(SelectIntoQueryCases))]
    public void SelectIntoQueryTest(SelectIntoQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IQuery, SqlServerRendererSettings, string> SqlInjectionAttackCases
    {
        get
        {
            TheoryData<IQuery, SqlServerRendererSettings, string> cases = new()
            {
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), EqualTo, "Dupont';--"),
                    new SqlServerRendererSettings(),

                    "DECLARE @p0 AS VARCHAR(8000) = 'Dupont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] = @p0)"
                },
                {
                    Select("id".Field())
                        .From("members")
                        .Where("username".Field(), Like, "Du[pont';--"),
                    new SqlServerRendererSettings(),

                    @"DECLARE @p0 AS VARCHAR(8000) = 'Du\[pont'';--';" +
                    "SELECT [id] FROM [members] WHERE ([username] LIKE @p0)"
                }
            };

            foreach (string naughtyString in TheNaughtyStrings.SQLInjection)
            {
                string escapedString = naughtyString
                    .Replace("\'", "''")
                    .Replace("[", "[");
                cases.Add(
                    Select("*").From("superheroes")
                        .Where("name".Field(), EqualTo, naughtyString),
                    new SqlServerRendererSettings (),
                    $"DECLARE @p0 AS VARCHAR(8000) = '{escapedString}';" +
                    "SELECT * FROM [superheroes] WHERE ([name] = @p0)"
                );
            }

            return cases;
        }
    }

    [Theory]
    [MemberData(nameof(SqlInjectionAttackCases))]
    public void PreventSqlInjectionAttack(IQuery query, SqlServerRendererSettings settings, string expectedString) => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<CreateViewQuery>, SqlServerRendererSettings, string> CreateViewCases
        => new()
        {
            {
                CreateView("active_users")
                .As(Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field() ))
                    .From("members")
                    .Where("IsActive".Field().EqualTo(true))
                    .Build()),
                new SqlServerRendererSettings (),
                "DECLARE @p0 AS BIT = 1;" +
                "CREATE VIEW [active_users] " +
                "AS SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE ([IsActive] = @p0)"
            },
            {
                CreateView("active_users")
                .As(Select(Concat("Firstname".Field(), " ".Literal(), "Lastname".Field() ))
                    .From("members")
                    .Where("IsActive".Field().In("val1", "val2"))
                    .Build()),
                new SqlServerRendererSettings (),
                "DECLARE @p0 AS VARCHAR(8000) = 'val1';" +
                "DECLARE @p1 AS VARCHAR(8000) = 'val2';" +
                "CREATE VIEW [active_users] " +
                "AS SELECT [Firstname] + ' ' + [Lastname] FROM [members] WHERE ([IsActive] IN (@p0, @p1))"
            },
            {
                CreateView("viewName")
                .As(Select("col1", "col2")
                    .From("table1").InnerJoin("table2".Table(), "table1.Id".Field().EqualTo("table2.Id".Field()))
                    .Where("IsActive".Field().In("val1", "val2"))
                    .Build()),
                new SqlServerRendererSettings (),
                "DECLARE @p0 AS VARCHAR(8000) = 'val1';" +
                "DECLARE @p1 AS VARCHAR(8000) = 'val2';" +
                "CREATE VIEW [viewName] " +
                "AS SELECT [col1], [col2] " +
                "FROM [table1] INNER JOIN [table2] ON ([table1].[Id] = [table2].[Id]) " +
                "WHERE ([IsActive] IN (@p0, @p1))"
            }
        };

    [Theory]
    [MemberData(nameof(CreateViewCases))]
    public void CreateViewTest(CreateViewQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    public static TheoryData<IBuild<InsertIntoQuery>, SqlServerRendererSettings, string> InsertIntoQueryCases
        => new()
        {
            {
                InsertInto("members")
                .Values(
                    "Firstname".Field().InsertValue("Bruce".Literal()),
                    "Lastname".Field().InsertValue("Wayne".Literal())
                ),
                new SqlServerRendererSettings(),
                "INSERT INTO [members] ([Firstname], [Lastname]) VALUES ('Bruce', 'Wayne')"
            }
        };

    [Theory]
    [MemberData(nameof(InsertIntoQueryCases))]
    public void InsertIntoQueryTest(InsertIntoQuery query, SqlServerRendererSettings settings, string expectedString)
        => IsQueryOk(query, settings, expectedString);

    private void IsQueryOk(IQuery query, SqlServerRendererSettings settings, string expectedString)
    {
        outputHelper.WriteLine($"{nameof(query)} : {query}");
        outputHelper.WriteLine($"{nameof(settings)} : {settings}");
        // Act
        string result = query.ForSqlServer(settings);

        // Assert
        result.Should().Be(expectedString);
    }
}