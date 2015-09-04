using System;
using System.Collections.Generic;
using NUnit.Framework;
using Queries.Builders;
using Queries.Parts;
using Queries.Parts.Columns;
using Queries.Parts.Joins;
using Queries.Renderers;

namespace Queries.Tests.Renderers
{
    //[TestFixture]
    //public class SqlServerRendererTest
    //{
    //    private SqlServerQueryBuilder _sqlBuilder;

    //    [SetUp]
    //    public void TestInitialize()
    //    {
    //        _sqlBuilder = new SqlServerQueryBuilder();
    //    }

    //    [TearDown]
    //    public void TestFinalize()
    //    {
    //        _sqlBuilder = null;
    //    }

    //    private class Cases
    //    {
    //        public IEnumerable<ITestCaseData> SelectTestCases
    //        {
    //            get
    //            {
    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Row_Number() OVER (ORDER BY Browser)", "Id")
    //                                                  .Select("Browser", "[Name]")
    //                                                  .Select(AggregateType.Count, "*", "[Count]")
    //                                                  .From("dbo.DatacarMonitor", "dm"))
    //                                                  .Returns("SELECT Row_Number() OVER (ORDER BY Browser) AS [Id], [Browser] AS [Name], COUNT(*) AS [Count] FROM [dbo].[DatacarMonitor] [dm] GROUP BY [Browser]")
    //                                                  .SetName("Select with generating row number");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                    .Select("ROW_NUMBER() OVER(ORDER BY Issue1)", "Id")
    //                    .Select("s.Id", "SurveyId")
    //                    .Select("usr.Issue1", "[Name]")
    //                    .Select("s.Country", "Country")
    //                    .Select("sc.SynergyCode", "SynergyCode")
    //                    .Select(AggregateType.Count, "*", "[Count]")
    //                    .From("dbo.UserSurveyResult", "usr")
    //                    .InnerJoin("dbo.Survey", "s",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("s.Id", WhereClauseOperator.EqualTo,
    //                            "usr.SurveyId"))
    //                    .InnerJoin("dbo.UserSynergyCompany", "usc", new WhereClauseBuilder(WhereLogic.And)
    //                        .AddClause("usc.UserId", WhereClauseOperator.EqualTo, "usr.UserId")
    //                        .AddClause("usc.IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean))
    //                    .InnerJoin("dbo.SynergyCompany", "sc",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.SynergyCompanyId",
    //                            WhereClauseOperator.EqualTo, "sc.Id"))
    //                    ).Returns("SELECT ROW_NUMBER() OVER(ORDER BY Issue1) AS [Id], " +
    //                              "[s].[Id] AS [SurveyId], [usr].[Issue1] AS [Name], [s].[Country] AS [Country], [sc].[SynergyCode] AS [SynergyCode], COUNT(*) AS [Count] " +
    //                              "FROM [dbo].[UserSurveyResult] [usr] " +
    //                              "INNER JOIN [dbo].[Survey] AS [s] ON [s].[Id] = usr.SurveyId " +
    //                              "INNER JOIN [dbo].[UserSynergyCompany] AS [usc] ON [usc].[UserId] = usr.UserId AND [usc].[IsAttachment] = 1 " +
    //                              "INNER JOIN [dbo].[SynergyCompany] AS [sc] ON [usc].[SynergyCompanyId] = sc.Id GROUP BY [s].[Id], [usr].[Issue1], [s].[Country], [sc].[SynergyCode]")
    //                    .SetName("Row_Number");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("OrderID")
    //                                                  .Select("CustomerName")
    //                                                  .Select("OrderDate")
    //                                                  .From("Orders")
    //                                                  .InnerJoin("Customers",
    //                                                             new WhereClauseBuilder(WhereLogic.And).AddClause(
    //                                                                 new WhereClause("Orders.CustomerId",
    //                                                                                 WhereClauseOperator.EqualTo,
    //                                                                                 "Customers.CustomerId")))
    //                                                  .LeftJoin("Clients",
    //                                                            new WhereClauseBuilder(WhereLogic.And).AddClause(
    //                                                                new WhereClause("Customers.ClientId",
    //                                                                                WhereClauseOperator.EqualTo,
    //                                                                                "Clients.ClientId"))))
    //                    .Returns("SELECT [OrderID], [CustomerName], [OrderDate] FROM [Orders] INNER JOIN [Customers] ON [Orders].[CustomerId] = Customers.CustomerId LEFT JOIN [Clients] ON [Customers].[ClientId] = Clients.ClientId")
    //                    .SetName("Select with 2 or more 'joins'");


    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("OrderID")
    //                                                  .Select("CustomerName")
    //                                                  .Select("OrderDate")
    //                                                  .From("Orders")
    //                                                  .InnerJoin("Customers",
    //                                                             new WhereClauseBuilder(WhereLogic.And).AddClause(
    //                                                                 new WhereClause("Orders.CustomerId",
    //                                                                                 WhereClauseOperator.EqualTo,
    //                                                                                 "Customers.CustomerId"))))
    //                    .Returns(
    //                        "SELECT [OrderID], [CustomerName], [OrderDate] FROM [Orders] INNER JOIN [Customers] ON [Orders].[CustomerId] = Customers.CustomerId")
    //                    .SetName("SelectQueryBuilder With One Join");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select( AggregateType.Count, "Column 2", "c2")
    //                                                  .Select( AggregateType.Sum, "Column 3", String.Empty)
    //                                                  .From("Table", "t"))
    //                    .Returns(
    //                        "SELECT [Column 1] AS [c1], COUNT([Column 2]) AS [c2], SUM([Column 3]) FROM [Table] [t] GROUP BY [Column 1]")
    //                    .SetName("SelectQueryBuilder With Many Aggregated Columns And No Flat Column");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Aggregate("Column 2", "c2", AggregateType.Count)
    //                                                  .Aggregate("Column 3", String.Empty, AggregateType.Sum)
    //                                                  .From("Table", "t"))
    //                    .Returns(
    //                        "SELECT [Column 1] AS [c1], COUNT([Column 2]) AS [c2], SUM([Column 3]) FROM [Table] [t] GROUP BY [Column 1]")
    //                    .SetName("SelectQueryBuilder With Many Aggregated Columns And One Flat Column");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select("Column 2", "c2")
    //                                                  .From("Table", "t"))
    //                    .Returns("SELECT [Column 1] AS [c1], [Column 2] AS [c2] FROM [Table] [t]")
    //                    .SetName("Select With NoCriteria");

    //                yield return new TestCaseData(new SelectQueryBuilder()).Returns(String.Empty).SetName("Select Empty Query");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select("Column 2", "c2")
    //                                                  .From("Table", "t")
    //                                                  .Where(new WhereClauseBuilder(WhereLogic.And)))
    //                    .Returns("SELECT [Column 1] AS [c1], [Column 2] AS [c2] FROM [Table] [t]")
    //                    .SetName("Select With EmptyCriteria");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Id")
    //                                                  .Select("Name")
    //                                                  .Select("SynergyCode")
    //                                                  .Select(new SelectQueryBuilder()
    //                                                              .Select(new AggregateColumn(AggregateType.Count, "*",
    //                                                                                    "NbTotalAccount"))
    //                                                              .From("dbo.UserSynergyCompany", "usc")
    //                                                              .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                                                         .AddClause("SynergyCompanyId",
    //                                                                                    WhereClauseOperator.EqualTo,
    //                                                                                    "sc.Id")
    //                                                                         .AddClause("IsAttachment",
    //                                                                                    WhereClauseOperator.EqualTo, "1",
    //                                                                                    DbType.Boolean)
    //                                                              )
    //                                                          , "NbTotalAccount")
    //                                                  .From("dbo.SynergyCompany", "sc"))
    //                    .Returns("SELECT [Id], [Name], [SynergyCode], (SELECT COUNT(*) AS [NbTotalAccount] FROM [dbo].[UserSynergyCompany] [usc] WHERE [SynergyCompanyId] = sc.Id AND [IsAttachment] = 1) AS [NbTotalAccount] FROM [dbo].[SynergyCompany] [sc]")
    //                    .SetName("Select With Columns Based On Select");


    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select("Column 2", "c2")
    //                                                  .From("Table", "t")
    //                                                  .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                                             .AddClause(new WhereClause("VersionObsolete",
    //                                                                                        WhereClauseOperator.EqualTo,
    //                                                                                        "0", DbType.Boolean))
    //                                                  ))
    //                    .Returns(
    //                        "SELECT [Column 1] AS [c1], [Column 2] AS [c2] FROM [Table] [t] WHERE [VersionObsolete] = 0")
    //                    .SetName("NoNestedCriteria");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select("Column 2", "c2")
    //                                                  .From("Table", "t")
    //                                                  .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                                             .AddClause(new WhereClause("VersionObsolete",
    //                                                                                        WhereClauseOperator.EqualTo,
    //                                                                                        "0"))
    //                                                             .AddClause(new WhereClauseBuilder(WhereLogic.Or)
    //                                                                            .AddClause(new WhereClause("IdProduit",
    //                                                                                                       WhereClauseOperator
    //                                                                                                           .EqualTo,
    //                                                                                                       "3",
    //                                                                                                       DbType.Int32))
    //                                                                            .AddClause(new WhereClause("IdProduit",
    //                                                                                                       WhereClauseOperator
    //                                                                                                           .EqualTo,
    //                                                                                                       "1",
    //                                                                                                       DbType.Int32))
    //                                                             )
    //                                                  ))
    //                    .Returns(
    //                        "SELECT [Column 1] AS [c1], [Column 2] AS [c2] FROM [Table] [t] WHERE [VersionObsolete] = 0 AND ([IdProduit] = 3 OR [IdProduit] = 1)")
    //                    .SetName("SelectQueryBuilder With Nested Criteria");


    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Aggregate("Column 2", "c2", AggregateType.Count)
    //                                                  .From("Table", "t"))
    //                    .Returns("SELECT COUNT([Column 2]) AS [c2] FROM [Table] [t]")
    //                    .SetName("SelectQueryBuilder With One Aggregated Column And No Flat Column");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Select("Column 2", "c2")
    //                                                  .From("Table", "t")
    //                                                  .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                                             .AddClause(new WhereClause("VersionObsolete", WhereClauseOperator.EqualTo, "0"))
    //                                                             .AddClause(new WhereClauseBuilder(WhereLogic.Or)
    //                                                                    .AddClause(new WhereClause("IdProduit", WhereClauseOperator.EqualTo, "3", DbType.Int32))
    //                                                                    .AddClause(new WhereClause("IdProduit", WhereClauseOperator.EqualTo, "1", DbType.Int32)))
    //                                                                    .AddClause(new WhereClauseBuilder(WhereLogic.Or)
    //                                                                        .AddClause(new WhereClause("ProductName", WhereClauseOperator.EqualTo, "Datacar DMS", DbType.String))
    //                                                                        .AddClause(new WhereClause("ProductName", WhereClauseOperator.EqualTo, "Datacar CRM", DbType.String))
    //                                                             )))
    //                    .Returns("SELECT [Column 1] AS [c1], [Column 2] AS [c2] " +
    //                             "FROM [Table] [t] " +
    //                             "WHERE [VersionObsolete] = 0 AND ([IdProduit] = 3 OR [IdProduit] = 1) AND ([ProductName] = 'Datacar DMS' OR [ProductName] = 'Datacar CRM')")
    //                    .SetName("SelectQueryBuilder With many nested criteria");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                                                  .Select("Column 1", "c1")
    //                                                  .Aggregate("Column 2", "c2", AggregateType.Count)
    //                                                  .From("Table", "t"))
    //                    .Returns("SELECT [Column 1] AS [c1], COUNT([Column 2]) AS [c2] FROM [Table] [t] GROUP BY [Column 1]")
    //                    .SetName("SelectQueryBuilder With One Aggregated Column And One Flat Column");


    //                yield return new TestCaseData(new SelectQueryBuilder(SelectQueryBuilder.SelectionMode.Top)
    //                                                  .Select("Country")
    //                                                  .From("User", "u")
    //                                                  .InnerJoin(new TableTerm("UserSynergyCompany", "usc"),
    //                                                             new WhereClauseBuilder(WhereLogic.And).AddClause(
    //                                                                 "u.Id", WhereClauseOperator.EqualTo, "usc.UserId"))
    //                                                  .OrderBy("IsAdmin", SortDirection.Descending))
    //                    .Returns(
    //                        "SELECT TOP 1 [Country] FROM [User] [u] INNER JOIN [UserSynergyCompany] AS [usc] ON [u].[Id] = usc.UserId ORDER BY [IsAdmin] DESC")
    //                        .SetName("Select with SelectionMode");


    //                yield return new TestCaseData(new SelectQuery()
    //                    .Select("dm.Id")
    //                    .Select("Description")
    //                    .Select("Browser")
    //                    .Select("m.Name", "MonitorName")
    //                    .Select("n.Name", "NavigationName")
    //                    .Select(new IsNullColumn("comp.Name", "''", "Company_Name"))
    //                    .Select(new IsNullColumn("comp.SynergyCode", "''", "Company_SynergyCode"))
    //                    .Select(new IsNullColumn("comp.Id", "''", "Company_Id"))
    //                    .Select("comp.CreatedDate", "Company_CreatedDate")
    //                    .Select("comp.CreatedBy", "Company_CreatedBy")
    //                    .Select("comp.UpdatedDate", "Company_UpdatedDate")
    //                    .Select("comp.UpdatedBy", "Company_UpdatedBy")
    //                    .Select("[Date]", "MonitorDate")
    //                    .Select("u.Id", "User_Id")
    //                    .Select("u.Language", "User_Language")
    //                    .Select("u.Country", "User_Country")
    //                    .Select("s.Name", "User_Service")
    //                    .Select("u.Username", "User_Username")
    //                    .Select("u.Firstname", "User_Firstname")
    //                    .Select("u.Lastname", "User_Lastname")
    //                    .Select("Year([Date])", "[Year]")
    //                    .Select("Month([date])", "[Month]")
    //                    .Select("Day([Date])", "[Day]")
    //                    .From("dbo.DatacarMonitor", "dm")
    //                    .InnerJoin("dbo.[User]", "u",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("dm.UserId", WhereClauseOperator.EqualTo,
    //                            "u.Id"))
    //                    .InnerJoin("dbo.MonitorType", "m",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("dm.MonitorTypeId",
    //                            WhereClauseOperator.EqualTo, "m.Id"))
    //                    .InnerJoin("dbo.NavigationType", "n",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("dm.NavigationTypeId",
    //                            WhereClauseOperator.EqualTo, "n.Id"))
    //                    .InnerJoin("dbo.Service", "s",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("u.ServiceId", WhereClauseOperator.EqualTo,
    //                            "s.Id"))
    //                    .InnerJoin("dbo.UserSynergyCompany", "usc",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId", WhereClauseOperator.EqualTo,
    //                            "dm.UserId")
    //                            .AddClause("usc.IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean))
    //                    .InnerJoin("dbo.SynergyCompany", "comp",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.SynergyCompanyId",
    //                            WhereClauseOperator.EqualTo, "comp.Id"))
    //                    )
    //                    .Returns(
    //                        "SELECT [dm].[Id], [Description], [Browser], [m].[Name] AS [MonitorName], [n].[Name] AS [NavigationName], ISNULL([comp].[Name],'') AS [Company_Name], ISNULL([comp].[SynergyCode],'') AS [Company_SynergyCode], ISNULL([comp].[Id],'') AS [Company_Id], [comp].[CreatedDate] AS [Company_CreatedDate], [comp].[CreatedBy] AS [Company_CreatedBy], [comp].[UpdatedDate] AS [Company_UpdatedDate], [comp].[UpdatedBy] AS [Company_UpdatedBy], [Date] AS [MonitorDate], [u].[Id] AS [User_Id], [u].[Language] AS [User_Language], [u].[Country] AS [User_Country], [s].[Name] AS [User_Service], [u].[Username] AS [User_Username], [u].[Firstname] AS [User_Firstname], [u].[Lastname] AS [User_Lastname], Year([Date]) AS [Year], Month([date]) AS [Month], Day([Date]) AS [Day] " +
    //                        "FROM [dbo].[DatacarMonitor] [dm] " +
    //                        "INNER JOIN [dbo].[User] AS [u] ON [dm].[UserId] = u.Id INNER JOIN [dbo].[MonitorType] AS [m] ON [dm].[MonitorTypeId] = m.Id " +
    //                        "INNER JOIN [dbo].[NavigationType] AS [n] ON [dm].[NavigationTypeId] = n.Id " +
    //                        "INNER JOIN [dbo].[Service] AS [s] ON [u].[ServiceId] = s.Id " +
    //                        "INNER JOIN [dbo].[UserSynergyCompany] AS [usc] ON [usc].[UserId] = dm.UserId AND [usc].[IsAttachment] = 1 " +
    //                        "INNER JOIN [dbo].[SynergyCompany] AS [comp] ON [usc].[SynergyCompanyId] = comp.Id")
    //                    .SetName("Select with Reserved keywords in columns definitions");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                    .Select("ROW_NUMBER() OVER (ORDER BY comp.SynergyCode)", "Id")
    //                    .Select(AggregateType.Count, "DISTINCT usc.UserId", "[COUNT]")
    //                    .Select("u.Country")
    //                    .Select("comp.SynergyCode")
    //                    .Select("DATEPART(HOUR, [Date])", "[HOUR]")
    //                    .Select("CAST([Date] as DATE)", "DateOfCount")
    //                    .From("DatacarMonitor", "dm")
    //                    .InnerJoin("UserSynergyCompany", "usc",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId", WhereClauseOperator.EqualTo,
    //                            "dm.UserId")
    //                            .AddClause("usc.IsAttachment", WhereClauseOperator.EqualTo, 1, DbType.Boolean))
    //                    .InnerJoin("[User]", "u",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId", WhereClauseOperator.EqualTo,
    //                            "u.Id"))
    //                    .InnerJoin("SynergyCompany", "comp",
    //                        new WhereClauseBuilder(WhereLogic.And).AddClause("usc.SynergyCompanyId",
    //                            WhereClauseOperator.EqualTo, "comp.Id"))
    //                    )
    //                    .Returns(
    //                        "SELECT ROW_NUMBER() OVER (ORDER BY comp.[SynergyCode]) AS [Id], [u].[Country], [comp].[SynergyCode], DATEPART(HOUR, [Date]) AS [HOUR], CAST([Date] as DATE) AS [DateOfCount], COUNT(DISTINCT usc.[UserId]) AS [COUNT] " +
    //                        "FROM [DatacarMonitor] [dm] INNER JOIN [UserSynergyCompany] AS [usc] ON [usc].[UserId] = dm.UserId AND [usc].[IsAttachment] = 1 " +
    //                        "INNER JOIN [User] AS [u] ON [usc].[UserId] = u.Id " +
    //                        "INNER JOIN [SynergyCompany] AS [comp] ON [usc].[SynergyCompanyId] = comp.Id " +
    //                        "GROUP BY [u].[Country], [comp].[SynergyCode], DATEPART(HOUR, [Date]), CAST([Date] as DATE)")
    //                    .SetName("Select with ROW_NUMBER bug case");

    //                yield return new TestCaseData(new SelectQueryBuilder()
    //                    .Select("*")
    //                    .Select(new SelectQueryBuilder()
    //                        .Select(AggregateType.Count, "*")
    //                        .From("[dbo].[User]", "u")
    //                        .InnerJoin("UserSynergyCompany", "usc",
    //                            new WhereClauseBuilder(WhereLogic.And).AddClause("u.Id", WhereClauseOperator.EqualTo,
    //                                "usc.UserId"))
    //                        .InnerJoin("SynergyCompany", "s",
    //                            new WhereClauseBuilder(WhereLogic.And).AddClause("usc.SynergyCompanyId",
    //                                WhereClauseOperator.EqualTo,
    //                                "s.Id"))
    //                        .Where(new WhereClauseBuilder(WhereLogic.And).AddClause("s.SynergyCode",
    //                            WhereClauseOperator.EqualTo,
    //                            "A.SynergyCode")
    //                            .AddClause("usc.IsAttachment", WhereClauseOperator.EqualTo, 1, DbType.Boolean)),
    //                        "TotalUserCount")
    //                    .From(new SelectQueryBuilder()
    //                        .Select("row_number() over (order by synergyCode)", "Id")
    //                        .Select("u.Country")
    //                        .Select(AggregateType.Count, "DISTINCT dm.UserId", "ActiveUsersCount")
    //                        .Select("SynergyCode")
    //                        .Select("CAST([Date] AS Date)", "DateOfCount")
    //                        .From("dbo.DatacarMonitor", "dm")
    //                        .InnerJoin("UserSynergyCompany", "usc", new WhereClauseBuilder(WhereLogic.And)
    //                            .AddClause("usc.UserId", WhereClauseOperator.EqualTo, "dm.UserId")
    //                            .AddClause("usc.IsAttachment", WhereClauseOperator.EqualTo, 1, DbType.Boolean))
    //                        .InnerJoin("User", "u",
    //                            new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId",
    //                                WhereClauseOperator.EqualTo,
    //                                "u.Id"))
    //                        .InnerJoin("SynergyCompany", "comp",
    //                            new WhereClauseBuilder(WhereLogic.And).AddClause("usc.SynergyCompanyId",
    //                                WhereClauseOperator.EqualTo, "comp.Id")), "A"))
    //                    .Returns(
    //                        "SELECT *, " +
    //                        "(SELECT COUNT(*) FROM [dbo].[User] [u] " +
    //                        "INNER JOIN [UserSynergyCompany] AS [usc] ON [u].[Id] = usc.UserId " +
    //                        "INNER JOIN [SynergyCompany] AS [s] ON [usc].[SynergyCompanyId] = s.Id " +
    //                        "WHERE [s].[SynergyCode] = A.SynergyCode AND [usc].[IsAttachment] = 1) AS [TotalUserCount] " +
    //                        "FROM " +
    //                        "(SELECT row_number() over (order by synergyCode) AS [Id], [u].[Country], [SynergyCode], CAST([Date] AS Date) AS [DateOfCount], COUNT(DISTINCT dm.[UserId]) AS [ActiveUsersCount] " +
    //                        "FROM [dbo].[DatacarMonitor] [dm] " +
    //                        "INNER JOIN [UserSynergyCompany] AS [usc] ON [usc].[UserId] = dm.UserId AND [usc].[IsAttachment] = 1 " +
    //                        "INNER JOIN [User] AS [u] ON [usc].[UserId] = u.Id " +
    //                        "INNER JOIN [SynergyCompany] AS [comp] ON [usc].[SynergyCompanyId] = comp.Id " +
    //                        "GROUP BY [u].[Country], [SynergyCode], CAST([Date] AS Date)) [A]")
    //                    .SetName("Select with FROM a select as a table");
    //            }
    //        }

    //        public IEnumerable<ITestCaseData> InsertTestCases
    //        {
    //            get
    //            {
    //                yield return new TestCaseData(
    //                    new InsertQueryBuilder()
    //                        .Into("Table")
    //                        .Value("Field", "data"))
    //                    .Returns("INSERT INTO [Table]([Field]) VALUES('data')")
    //                    .SetName("Insert Into One Table One Field");

    //                yield return new TestCaseData(
    //                    new InsertQueryBuilder()
    //                        .Into("Table")
    //                        .Value("Field1", "data1")
    //                        .Value("Field2", "data2")
    //                        .Value("Field3", "data3"))
    //                    .Returns("INSERT INTO [Table]([Field1], [Field2], [Field3]) VALUES('data1', 'data2', 'data3')")
    //                    .SetName("Insert Into One Table Many Fields");

    //                yield return new TestCaseData(
    //                    new InsertQueryBuilder()
    //                        .Into("Destination")
    //                        .From("Source")
    //                    )
    //                    .Returns("INSERT INTO [Destination] SELECT * FROM [Source]")
    //                    .SetName("Insert Into One Table From Another Table");

    //                yield return new TestCaseData(
    //                    new InsertQueryBuilder()
    //                        .Into("Destination")
    //                        .From(
    //                            new SelectQueryBuilder()
    //                                .From("source")
    //                                .Select(new SelectColumn("source1"))
    //                                .Select(new SelectColumn("source2"))
    //                        )
    //                    )
    //                    .Returns("INSERT INTO [Destination] SELECT [source1], [source2] FROM [source]")
    //                    .SetName("Insert Into One Table From Select With Two Or More Columns");

    //                yield return new TestCaseData(new InsertQueryBuilder()
    //                    .Into("Destination", new[]
    //                    {
    //                        new InsertColumn("dest1"),
    //                        new InsertColumn("dest2"),

    //                    })
    //                    .From(
    //                        new SelectQueryBuilder()
    //                            .From("source")
    //                            .Select(new SelectColumn("source1"))
    //                            .Select(new SelectColumn("source2"))
    //                    ))
    //                    .Returns("INSERT INTO [Destination]([dest1],[dest2]) SELECT [source1], [source2] FROM [source]");
    //            }
    //        }

    //        public IEnumerable<ITestCaseData> CreateViewTestCases
    //        {
    //            get
    //            {
    //                yield return new TestCaseData(
    //                    new CreateViewBuilder("vDatacarMonitors", new SelectQueryBuilder()
    //                        .From("source")
    //                        .Select(new SelectColumn("source1"))
    //                        .Select(new SelectColumn("source2"))
    //                        ))
    //                    .Returns("CREATE VIEW [vDatacarMonitors] AS SELECT [source1], [source2] FROM [source]")
    //                    .SetName("Create a view from a select query");

    //                yield return new TestCaseData(

    //                    new CreateViewBuilder("vDatacarMonitorDetail", new SelectQueryBuilder()
    //                        .Select("Id")
    //                        .Select("Name")
    //                        .Select("SynergyCode")
    //                        .Select(new SelectQueryBuilder()
    //                            .Select(new AggregateColumn(AggregateType.Count, "*", "NbTotalAccount"))
    //                            .From("dbo.UserSynergyCompany", "usc")
    //                            .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                .AddClause("SynergyCompanyId", WhereClauseOperator.EqualTo, "sc.Id")
    //                                .AddClause("IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean)),
    //                            "NbTotalAccount")
    //                        .Select(new SelectQueryBuilder()
    //                            .Select(new AggregateColumn(AggregateType.Count, "*", "NbDMSAccount"))
    //                            .From("dbo.UserSynergyCompany", "usc")
    //                            .InnerJoin(new TableTerm("dbo.UserApplication", "ua"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId",
    //                                    WhereClauseOperator.EqualTo, "ua.UserId"))
    //                            .InnerJoin(new TableTerm("dbo.Application", "app"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("ua.ApplicationId",
    //                                    WhereClauseOperator.EqualTo, "app.Id"))
    //                            .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                .AddClause("SynergyCompanyId", WhereClauseOperator.EqualTo, "sc.Id")
    //                                .AddClause("IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean)
    //                                .AddClause("app.Name", WhereClauseOperator.EqualTo, "DatacarDMS", DbType.String)), "NbDMSAccount")
    //                        .Select(new SelectQueryBuilder()
    //                            .Select(new AggregateColumn(AggregateType.Count, "*", "NbCRMAccount"))
    //                            .From("dbo.UserSynergyCompany", "usc")
    //                            .InnerJoin(new TableTerm("dbo.UserApplication", "ua"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId",
    //                                    WhereClauseOperator.EqualTo, "ua.UserId"))
    //                            .InnerJoin(new TableTerm("dbo.Application", "app"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("ua.ApplicationId",WhereClauseOperator.EqualTo, "app.Id"))
    //                            .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                .AddClause("SynergyCompanyId", WhereClauseOperator.EqualTo, "sc.Id")
    //                                .AddClause("IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean)
    //                                .AddClause("app.Name", WhereClauseOperator.EqualTo, "DatacarCRM", DbType.String)), "NbCRMAccount")
    //                        .Select(new SelectQueryBuilder()
    //                            .Select(new AggregateColumn(AggregateType.Count, "*", "NbDMSAccount"))
    //                            .From("dbo.UserSynergyCompany", "usc")
    //                            .InnerJoin(new TableTerm("dbo.UserApplication", "ua"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("usc.UserId",
    //                                    WhereClauseOperator.EqualTo, "ua.UserId"))
    //                            .InnerJoin(new TableTerm("dbo.Application", "app"),
    //                                new WhereClauseBuilder(WhereLogic.And).AddClause("ua.ApplicationId",
    //                                    WhereClauseOperator.EqualTo, "app.Id"))
    //                            .Where(new WhereClauseBuilder(WhereLogic.And)
    //                                .AddClause("SynergyCompanyId", WhereClauseOperator.EqualTo, "sc.Id")
    //                                .AddClause("IsAttachment", WhereClauseOperator.EqualTo, "1", DbType.Boolean)
    //                                .AddClause("app.Name", WhereClauseOperator.EqualTo, "DatacarBI",DbType.String)), "NbBIAccount")
    //                        .From("dbo.SynergyCompany", "sc"))

    //                    )
    //                    .Returns("CREATE VIEW [vDatacarMonitorDetail] AS " +
    //                             "SELECT [Id], [Name], [SynergyCode], " +
    //                             "(SELECT COUNT(*) AS [NbTotalAccount] FROM [dbo].[UserSynergyCompany] [usc] WHERE [SynergyCompanyId] = sc.Id AND [IsAttachment] = 1) AS [NbTotalAccount], " +
    //                             "(SELECT COUNT(*) AS [NbDMSAccount] FROM [dbo].[UserSynergyCompany] [usc] INNER JOIN [dbo].[UserApplication] AS [ua] ON [usc].[UserId] = ua.UserId INNER JOIN [dbo].[Application] AS [app] ON [ua].[ApplicationId] = app.Id WHERE [SynergyCompanyId] = sc.Id AND [IsAttachment] = 1 AND [app].[Name] = 'DatacarDMS') AS [NbDMSAccount], " +
    //                             "(SELECT COUNT(*) AS [NbCRMAccount] FROM [dbo].[UserSynergyCompany] [usc] INNER JOIN [dbo].[UserApplication] AS [ua] ON [usc].[UserId] = ua.UserId INNER JOIN [dbo].[Application] AS [app] ON [ua].[ApplicationId] = app.Id WHERE [SynergyCompanyId] = sc.Id AND [IsAttachment] = 1 AND [app].[Name] = 'DatacarCRM') AS [NbCRMAccount], " +
    //                             "(SELECT COUNT(*) AS [NbDMSAccount] FROM [dbo].[UserSynergyCompany] [usc] INNER JOIN [dbo].[UserApplication] AS [ua] ON [usc].[UserId] = ua.UserId INNER JOIN [dbo].[Application] AS [app] ON [ua].[ApplicationId] = app.Id WHERE [SynergyCompanyId] = sc.Id AND [IsAttachment] = 1 AND [app].[Name] = 'DatacarBI') AS [NbBIAccount] " +
    //                             "FROM [dbo].[SynergyCompany] [sc]")
    //                    .SetName("Create view with computed columns");



    //                    ;
    //            }
    //        }

    //        public IEnumerable<ITestCaseData> UpdateTestCases
    //        {
    //            get
    //            {

    //                yield return new TestCaseData(
    //                    "User",
    //                    new List<Tuple<string, QueryFieldValue>>
    //                        {
    //                            new Tuple<string, QueryFieldValue>("Name", new QueryFieldValue( "John Doe", DbType.String))
    //                        },

    //                    null, null)

    //                    .Returns("UPDATE [User] SET [Name] = 'John Doe'")
    //                    .SetName("Update one field with one value");



    //                yield return new TestCaseData(
    //                    "User",
    //                    new List<Tuple<string, QueryFieldValue>>
    //                        {
    //                            new Tuple<string, QueryFieldValue>("Name", new QueryFieldValue( "John Doe", DbType.String))
    //                        },

    //                    new List<Tuple<string, SelectQueryBuilder>>
    //                        {
    //                            new Tuple<string, SelectQueryBuilder>("Firstname", new SelectQueryBuilder().Select("Column1").From(new TableTerm("MemberShip", "m")))
    //                        }, null)

    //                    .Returns("UPDATE [User] SET [Name] = 'John Doe', [Firstname] = (SELECT [Column1] FROM [MemberShip] [m])")
    //                    .SetName("Update two fields one value of type string and other value computed from a select statement ");

    //                yield return new TestCaseData(
    //                    "User",
    //                    new List<Tuple<string, QueryFieldValue>>
    //                        {
    //                            new Tuple<string, QueryFieldValue>("Name", new QueryFieldValue( "John Doe", DbType.String))
    //                        },

    //                    new List<Tuple<string, SelectQueryBuilder>>
    //                        {
    //                            new Tuple<string, SelectQueryBuilder>("Firstname", new SelectQueryBuilder().Select("Column1").From(new TableTerm("MemberShip", "m")))
    //                        }, null)

    //                    .Returns("UPDATE [User] SET [Name] = 'John Doe', [Firstname] = (SELECT [Column1] FROM [MemberShip] [m])");
    //            }
    //        }

    //        //public IEnumerable<ITestCaseData> DeleteTestCases
    //        //{
    //        //    get
    //        //    {
    //        //        return new TestCaseData(new DeleteQueryBuilder("Table"));
    //        //    }
    //        //}
    //    }


    //    [Test]
    //    [TestCaseSource(typeof(Cases), "SelectTestCases")]
    //    public string Select(SelectQueryBuilder queryBuilder)
    //    {
    //        return _sqlBuilder.RenderSelect(queryBuilder);
    //    }

    //    [Test]
    //    [TestCaseSource(typeof(Cases), "InsertTestCases")]
    //    public string Insert(InsertQueryBuilder queryBuilder)
    //    {
    //        return _sqlBuilder.RenderInsert(queryBuilder);
    //    }

    //    [Test]
    //    [TestCaseSource(typeof(Cases), "CreateViewTestCases")]
    //    public string CreateView(CreateViewBuilder queryBuilder)
    //    {
    //        return _sqlBuilder.RenderCreateView(queryBuilder);
    //    }

    //    [Test]
    //    [TestCaseSource(typeof(Cases), "UpdateTestCases")]
    //    public string Update(string tablename, IEnumerable<Tuple<string, QueryFieldValue>> primaryValues, IEnumerable<Tuple<string, SelectQueryBuilder>> computedValues, WhereClauseBuilder whereBuilder)
    //    {
    //        UpdateQueryBuilder updateQueryBuilder = new UpdateQueryBuilder(tablename);

    //        if (primaryValues != null)
    //        {
    //            foreach (Tuple<string, QueryFieldValue> value in primaryValues)
    //            {
    //                updateQueryBuilder = updateQueryBuilder.Set(value.Item1, value.Item2);
    //            }
    //        }
    //        if (computedValues != null)
    //        {
    //            foreach (Tuple<string, SelectQueryBuilder> value in computedValues)
    //            {
    //                updateQueryBuilder = updateQueryBuilder.Set(value.Item1, value.Item2);
    //            }

    //        }
    //        if (whereBuilder != null)
    //        {
    //            updateQueryBuilder.Where(whereBuilder);
    //        }

    //        return _sqlBuilder.RenderUpdate(updateQueryBuilder);

    //    }
    //}


    public class RenderersTest
    {

        
        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectTestsCases
            {
                get
                {
                    #region SQL SERVER
                    yield return new TestCaseData(null, DatabaseType.SqlServer)
                                .SetName("Select null query")
                                .SetCategory("SQL SERVER")
                                .Returns(String.Empty);


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [Col1] FROM [Table] [t]");



                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT * FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = null,
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName("Select with no column specified and one table in the FROM part")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT * FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new[]
                        {
                            new Table(){Name = "dbo.Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }> FROM <Table { Name = ""dbo.Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [Col1] FROM [dbo].[Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"}
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table1", Alias = "t1"},
                            new Table(){Name = "Table2", Alias = "t2"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }>, <TableColumn{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [Col1], [Col2] FROM [Table1] [t1], [Table2] [t2]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1", Alias = "Alias"}
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"", Alias = ""Alias"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [Col1] AS [Alias] FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"},
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }>, <TableColumn{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [Col1], [Col2] FROM [Table] [t]");




                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min (new TableColumn() {Name = "Col1"})
                        },
                        From = new[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <Min(new TableColumn() {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min ("Col1")
                        },
                        From = new[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <new Min (""Col1"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1"})
                        },
                        From = new[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                        .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) FROM [Table] [t]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Alias"})
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Alias""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) AS [Alias] FROM [Table] [t]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new Max(new TableColumn(){Name = "Col2", Alias = "Maximum"})
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})>, <Min(TableColumn {Name = ""Col2"", Alias = ""Maximum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) AS [Minimum], MAX([Col2]) AS [Maximum] FROM [Table] [t]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new TableColumn(){Name = "Col2", Alias = "Maximum"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})>, <TableColumn {Name = ""Col2"", Alias = ""Maximum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("SQL SERVER")
                    .SetDescription("This should generate a GROUP BY clause")
                    .Returns("SELECT MIN([Col1]) AS [Minimum], [Col2] AS [Maximum] FROM [Table] [t] GROUP BY [Col2]");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },
                        Union = new[]{
                           new SelectQuery()
                           { 
                                Select = new IColumn[]
                                {
                                    new Min(new TableColumn(){Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new[]
                                {
                                    new Table(){Name = "Table2", Alias = "t2"}
                                }
                           }
                        }
                    }, DatabaseType.SqlServer)
                   .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }> UNION SELECT <Min(TableColumn {Name = ""Col2"", Alias = ""Minimum""})> FROM <Table { Name = ""Table2"", Alias = ""t2"" }>""")
                   .SetCategory("SQL SERVER")
                    .Returns("SELECT MIN([Col1]) AS [Minimum] FROM [Table] [t] UNION SELECT MIN([Col2]) AS [Minimum] FROM [Table2] [t2]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "civ_prenom"}
                        },

                        From = new[]
                        {
                            new Table(){ Name =  "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = TableColumn.From("civ_nom"),
                            Operator = WhereOperator.EqualTo,
                            Constraint = "dupont"
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn {Name = ""civ_prenom""}> FROM <Table { Name = ""t_civilite"", Alias = ""civ"" }> WHERE <WhereClause { Column = TableColumn.From(""civ_nom""),Operator = WhereOperator.EqualTo, Constraint = ""dupont""})>")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [civ_prenom] FROM [t_civilite] [civ] WHERE ([civ_nom] = 'dupont')");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            TableColumn.From("prenom")
                        },
                        From = new[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.Or,
                            Clauses = new List<IClause>()
                            {
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                            }
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  Logic = WhereLogic.Or, Clauses = new IClause[]{new WhereClause() {Column = new TableColumn(){ Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15}, new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}})")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE (([per_age] >= 15) OR ([per_age] < 18))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "durant"}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <TableColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  
                        Logic = WhereLogic.And, 
                        Clauses = new IClause[]{
                            new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""dupont""},
                                        new WhereClause(){Column = new TableColumn{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""durant""}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                } 
                        }
                    })")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE ((([per_nom] = 'dupont') OR ([per_nom] = 'durant')) AND (([per_age] >= 15) OR ([per_age] < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    }, DatabaseType.SqlServer)
                    .SetName("SELECT with WhereClause + Composite clause")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [prenom] FROM [t_person] [p] WHERE (([per_nom] = 'dupont') AND (([per_age] >= 15) OR ([per_age] < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new TableColumn(){Name = "p.Name", Alias = "Nom complet"},
                            new TableColumn() {Name = "c.Name", Alias = "Civilité"}
                        },
                        From = new[] { new Table() { Name = "person", Alias = "p" } },
                        Joins = new IJoin[]
                        {
                            new InnerJoin(new Table(){Name = "Civility", Alias = "c"}, 
                                new WhereClause(){ Column = new TableColumn(){Name = "c.Id"}, Operator = WhereOperator.EqualTo, Constraint = TableColumn.From("p.CivilityId")})
                        }
                    }, DatabaseType.SqlServer)
                    .SetName("Select with one inner join")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [p].[Name] AS [Nom complet], [c].[Name] AS [Civilité] FROM [person] [p] INNER JOIN [Civility] [c] ON ([c].[Id] = [p].[CivilityId])");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new TableColumn(){Name = "p.Name", Alias = "Nom complet"},
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]{new TableColumn() {Name = "c.Name", Alias = "Civilité"}},
                                    From = new [] {new Table(){Name = "Civility", Alias = "c"}},
                                    Where = new WhereClause(){Column = TableColumn.From("p.CivilityId"), Operator = WhereOperator.EqualTo, Constraint = TableColumn.From("c.Id")}
                                }
                            }
                        },
                        From = new[] { new Table() { Name = "person", Alias = "p" } },

                    }, DatabaseType.SqlServer)
                    .SetName("SELECT TableColumn col1, (SELECT TableColumn col2 FROM Table2 WHERE Table1.col = Table2.col) AS alias FROM Table1")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [p].[Name] AS [Nom complet], (SELECT [c].[Name] AS [Civilité] FROM [Civility] [c] WHERE ([p].[CivilityId] = [c].[Id])) FROM [person] [p]");


                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" }
                    }, DatabaseType.SqlServer)
                    .SetName("SELECT into from list of table")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT * INTO [destination] FROM [source]");

                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" }
                    }, DatabaseType.SqlServer)
                    .SetName("SELECT into from list of table")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT * INTO [destination] FROM [source]");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]
                                    {
                                        new LiteralColumn {Value = 1},
                                    }
                                }
                            }
                        }
                    }, DatabaseType.SqlServer)
                    .SetCategory("SQL SERVER")
                    .SetName(@"""SELECT <SelectColumn(
                        {
                            SelectQuery = new SelectQuery(){
                                Select = new IColumn[]
                                {
                                    new LiteralColumn {Value = 1},
                                }
                            }
                        }>""")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT (SELECT 1)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(TableColumn.From("firstname"), LiteralColumn.From(" "), TableColumn.From("lastname")){Alias = "fullname" }, 
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "t_person", Alias = "p"} 
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""SELECT <Concat {alias: ""fullname"", columns: new IColumn[]
                            {
                                TableColumn.From(""firstname""),
                                LiteralColumn.From("" ""),
                                TableColumn.From(""lastname"")
                            }> 
                        FROM <Table {Name = ""t_person"", Alias = ""p""}> ")
                    .SetCategory("SQL SERVER")
                    .Returns("SELECT [firstname] + ' ' + [lastname] AS [fullname] FROM [t_person] [p]");
                    
                    #endregion

                    #region POSTGRES
                    yield return new TestCaseData(null, DatabaseType.Postgres)
                                .SetName("Select null query")
                                .SetCategory("POSTGRES")
                                .Returns(String.Empty);


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }> 
                                FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""Col1"" FROM ""Table"" ""t""");



                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT * FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = null,
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName("Select with no column specified and one table in the FROM part")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT * FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "dbo.Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }> FROM <Table { Name = ""dbo.Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""Col1"" FROM ""dbo"".""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table1", Alias = "t1"},
                            new Table(){Name = "Table2", Alias = "t2"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }>, <TableColumn{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""Col1"", ""Col2"" FROM ""Table1"" ""t1"", ""Table2"" ""t2""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1", Alias = "Alias"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"", Alias = ""Alias"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""Col1"" ""Alias"" FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){Name = "Col1"},
                            new TableColumn(){Name = "Col2"},
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn{ Name = ""Col1"" }>, <TableColumn{ Name = ""Col2"" }> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""Col1"", ""Col2"" FROM ""Table"" ""t""");
                    
                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min (new TableColumn() {Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <Min(new TableColumn() {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min ("Col1")
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <new Min (""Col1"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1"})
                        },
                        From = new ITable[]
                        {
                            new Table() {Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                        .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                        .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Alias"})
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Alias""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") ""Alias"" FROM ""Table"" ""t""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new Max(new TableColumn(){Name = "Col2", Alias = "Maximum"})
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})>, <Min(TableColumn {Name = ""Col2"", Alias = ""Maximum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") ""Minimum"", MAX(""Col2"") ""Maximum"" FROM ""Table"" ""t""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                            new TableColumn(){Name = "Col2", Alias = "Maximum"}
                        },
                        From = new ITable[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },

                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})>, <TableColumn {Name = ""Col2"", Alias = ""Maximum"")> FROM <Table { Name = ""Table"", Alias = ""t"" }>""")
                    .SetCategory("POSTGRES")
                    .SetDescription("This should generate a GROUP BY clause")
                    .Returns(@"SELECT MIN(""Col1"") ""Minimum"", ""Col2"" ""Maximum"" FROM ""Table"" ""t"" GROUP BY ""Col2""");

                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Min(new TableColumn(){Name = "Col1", Alias = "Minimum"}),
                        },
                        From = new[]
                        {
                            new Table(){Name = "Table", Alias = "t"}
                        },
                        Union = new[]{
                           new SelectQuery()
                           { 
                                Select = new IColumn[]
                                {
                                    new Min(new TableColumn(){Name = "Col2", Alias = "Minimum"}),
                                },
                                From = new ITable[]
                                {
                                    new Table(){Name = "Table2", Alias = "t2"}
                                }
                           }
                        }
                    }, DatabaseType.Postgres)
                   .SetName(@"""SELECT <Min(TableColumn {Name = ""Col1"", Alias = ""Minimum""})> FROM <Table { Name = ""Table"", Alias = ""t"" }> UNION SELECT <Min(TableColumn {Name = ""Col2"", Alias = ""Minimum""})> FROM <Table { Name = ""Table2"", Alias = ""t2"" }>""")
                   .SetCategory("POSTGRES")
                    .Returns(@"SELECT MIN(""Col1"") ""Minimum"" FROM ""Table"" ""t"" UNION SELECT MIN(""Col2"") ""Minimum"" FROM ""Table2"" ""t2""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "civ_prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_civilite", Alias = "civ"}
                        },
                        Where = new WhereClause()
                        {
                            Column = TableColumn.From("civ_nom"),
                            Operator = WhereOperator.EqualTo,
                            Constraint = "dupont"
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn {Name = ""civ_prenom""}> FROM <Table { Name = ""t_civilite"", Alias = ""civ"" }> WHERE <WhereClause { Column = TableColumn.From(""civ_nom""),Operator = WhereOperator.EqualTo, Constraint = ""dupont""})>")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""civ_prenom"" FROM ""t_civilite"" ""civ"" WHERE (""civ_nom"" = 'dupont')");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            TableColumn.From("prenom")
                        },
                        From = new[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.Or,
                            Clauses = new List<IClause>()
                            {
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                new WhereClause() {Column = new TableColumn(){ Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                            }
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  Logic = WhereLogic.Or, Clauses = new IClause[]{new WhereClause() {Column = new TableColumn(){ Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15}, new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}})")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE ((""per_age"" >= 15) OR (""per_age"" < 18))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                        new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "durant"}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""SELECT <TableColumn.From(""prenom"")> FROM <Table { Name = ""t_person"", Alias = ""p"" }> WHERE <CompositeWhereClause {  
                        Logic = WhereLogic.And, 
                        Clauses = new IClause[]{
                            new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""dupont""},
                                        new WhereClause(){Column = new TableColumn{Name = ""per_nom""}, Operator = WhereOperator.EqualTo, Constraint = ""durant""}
                                    }
                                },
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = ""per_age""}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                } 
                        }
                    })")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE (((""per_nom"" = 'dupont') OR (""per_nom"" = 'durant')) AND ((""per_age"" >= 15) OR (""per_age"" < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new TableColumn(){ Name = "prenom"}
                        },

                        From = new ITable[]
                        {
                            new Table(){ Name =  "t_person", Alias = "p"}
                        },
                        Where = new CompositeWhereClause()
                        {
                            Logic = WhereLogic.And,
                            Clauses = new IClause[]
                            {
                                new WhereClause(){Column = new TableColumn{Name = "per_nom"}, Operator = WhereOperator.EqualTo, Constraint = "dupont"},
                                new CompositeWhereClause()
                                {
                                    Logic = WhereLogic.Or,
                                    Clauses = new IClause[]{
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.GreaterThanOrEqualTo, Constraint = 15},
                                        new WhereClause(){Column = new TableColumn{Name = "per_age"}, Operator = WhereOperator.LessThan, Constraint = 18}
                                    }
                                }
                            }
                        }
                    }, DatabaseType.Postgres)
                    .SetName("SELECT with WhereClause + Composite clause")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""prenom"" FROM ""t_person"" ""p"" WHERE ((""per_nom"" = 'dupont') AND ((""per_age"" >= 15) OR (""per_age"" < 18)))");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new TableColumn(){Name = "p.Name", Alias = "Nom complet"},
                            new TableColumn() {Name = "c.Name", Alias = "Civilité"}
                        },
                        From = new ITable[] { new Table() { Name = "person", Alias = "p" } },
                        Joins = new IJoin[]
                        {
                            new InnerJoin(new Table(){Name = "Civility", Alias = "c"}, 
                                new WhereClause(){ Column = new TableColumn(){Name = "c.Id"}, Operator = WhereOperator.EqualTo, Constraint = TableColumn.From("p.CivilityId")})
                        }
                    }, DatabaseType.Postgres)
                    .SetName("Select with one inner join")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""p"".""Name"" ""Nom complet"", ""c"".""Name"" ""Civilité"" FROM ""person"" ""p"" INNER JOIN ""Civility"" ""c"" ON (""c"".""Id"" = ""p"".""CivilityId"")");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new List<IColumn>()
                        {
                            new TableColumn(){Name = "p.Name", Alias = "Nom complet"},
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]{new TableColumn() {Name = "c.Name", Alias = "Civilité"}},
                                    From = new ITable[] {new Table(){Name = "Civility", Alias = "c"}},
                                    Where = new WhereClause(){Column = TableColumn.From("p.CivilityId"), Operator = WhereOperator.EqualTo, Constraint = TableColumn.From("c.Id")}
                                }
                            }
                        },
                        From = new[] { new Table() { Name = "person", Alias = "p" } },

                    }, DatabaseType.Postgres)
                    .SetName("SELECT TableColumn col1, (SELECT TableColumn col2 FROM Table2 WHERE Table1.col = Table2.col) AS alias FROM Table1")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT ""p"".""Name"" ""Nom complet"", (SELECT ""c"".""Name"" ""Civilité"" FROM ""Civility"" ""c"" WHERE (""p"".""CivilityId"" = ""c"".""Id"")) FROM ""person"" ""p""");


                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" } 
                    }, DatabaseType.Postgres)
                    .SetName("SELECT into from list of table")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT * INTO ""destination"" FROM ""source""");

                    yield return new TestCaseData(new SelectIntoQuery()
                    {
                        Into = new Table() { Name = "destination" },
                        From = new Table() { Name = "source" }
                    }, DatabaseType.Postgres)
                    .SetName("SELECT into <Table> from <Table>")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT * INTO ""destination"" FROM ""source""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new SelectColumn()
                            {
                                SelectQuery = new SelectQuery()
                                {
                                    Select = new IColumn[]
                                    {
                                        new LiteralColumn {Value = 1},
                                    }
                                }
                            }
                        }
                    }, DatabaseType.Postgres)
                        .SetCategory("POSTGRES")
                        .SetName(@"""SELECT <SelectColumn(
                        {
                            SelectQuery = new SelectQuery(){
                                Select = new IColumn[]
                                {
                                    new LiteralColumn {Value = 1},
                                }
                            }
                        }>""")
                        .SetCategory("POSTGRES")
                        .Returns("SELECT (SELECT 1)");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        Select = new IColumn[]
                        {
                            new Concat(
                                TableColumn.From("firstname"),
                                LiteralColumn.From(" "),
                                TableColumn.From("lastname")
                                ) {Alias = "fullname"},
                        },
                        From = new[]
                        {
                            new Table() {Name = "t_person", Alias = "p"}
                        }
                    }, DatabaseType.Postgres)
                        .SetName(@"""SELECT <Concat (TableColumn.From(""firstname""), LiteralColumn.From("" ""),TableColumn.From(""lastname"")
                            }> 
                        FROM <Table {Name = ""t_person"", Alias = ""p""}> ")
                        .SetCategory("POSTGRES")
                        .Returns(@"SELECT ""firstname"" || ' ' || ""lastname"" ""fullname"" FROM ""t_person"" ""p""");


                    yield return new TestCaseData(new SelectQuery()
                    {
                        From = new ITable[]
                        {
                            new SelectTable()
                            {
                                Select = new SelectQuery()
                                {
                                    Select = new IColumn[]
                                    {
                                        TableColumn.From("firstname"),
                                        TableColumn.From("lastname")
                                    },
                                    From = new ITable[]
                                    {
                                        new Table(){Name = "members"}
                                    }
                                }
                            }
                        }
                    }, DatabaseType.Postgres)
                    .SetName("SELECT FROM SelectTable")
                    .SetCategory("POSTGRES")
                    .Returns(@"SELECT * FROM SELECT ""firstname"", ""lastname"" FROM ""members""");

                    #endregion


                }
            }
            
            public IEnumerable<ITestCaseData> RenderUpdateTestCases
            {
                get
                {
                    #region SQL SERVER
                    yield return new TestCaseData(new UpdateQuery()
            {
                Table = new Table { Name = "Table" },
                Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = TableColumn.From("col1")}
                        }
            }, DatabaseType.SqlServer)
            .SetName("\"UPDATE <tablename> SET <destination> = <source>\" where <destination> and <source> are table columns")
            .SetCategory("SQL SERVER")
            .Returns("UPDATE [Table] SET [col2] = [col1]");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = "col1"}
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# string")
                    .SetCategory("SQL SERVER")
                    .Returns("UPDATE [Table] SET [col2] = 'col1'");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = 1}
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# positive integer")
                    .SetCategory("SQL SERVER")
                    .Returns("UPDATE [Table] SET [col2] = 1");


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = -1}
                        }
                    }, DatabaseType.SqlServer)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# negative integer")
                    .SetCategory("SQL SERVER")
                    .Returns("UPDATE [Table] SET [col2] = -1");
                    
                    #endregion


                    #region POSTGRES


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = TableColumn.From("col1")}
                        }
                    }, DatabaseType.Postgres)
            .SetName("\"UPDATE <tablename> SET <destination> = <source>\" where <destination> and <source> are table columns")
            .SetCategory("POSTGRES")
            .Returns(@"UPDATE ""Table"" SET ""col2"" = ""col1""");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = "col1"}
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# string")
                    .SetCategory("POSTGRES")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = 'col1'");

                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = 1}
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# positive integer")
                    .SetCategory("POSTGRES")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = 1");


                    yield return new TestCaseData(new UpdateQuery()
                    {
                        Table = new Table { Name = "Table" },
                        Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = -1}
                        }
                    }, DatabaseType.Postgres)
                    .SetName(@"""UPDATE <tablename> SET <destination> = <source>"" where <destination> is a table column and <source> is a c# negative integer")
                    .SetCategory("POSTGRES")
                    .Returns(@"UPDATE ""Table"" SET ""col2"" = -1");

                    yield return new TestCaseData(new UpdateQuery()
                                {
                                    Table = new Table { Name = "Table" },
                                    Set = new[]
                        {
                            new UpdateFieldValue(){ Destination = TableColumn.From("col2"), Source = TableColumn.From("col1")}
                        }
                                }, DatabaseType.Postgres)
                                .SetName("\"UPDATE <tablename> SET <destination> = <source>\" where <destination> and <source> are table columns")
                                .SetCategory("POSTGRES")
                                .Returns(@"UPDATE ""Table"" SET ""col2"" = ""col1""");
                    
                    #endregion

                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }


        [TestCaseSource(typeof(Cases), "SelectTestsCases")]
        public string RenderSelect(SelectQueryBase selectQuery, DatabaseType databaseType)
        {
            ISqlRenderer renderer;
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.SqlServerCompact:
                    renderer = new SqlServerRenderer();
                    break;
                case DatabaseType.Mysql:
                case DatabaseType.Postgres:
                    renderer = new PostgresqlRenderer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("databaseType");
            }


            return renderer.Render(selectQuery);
        }

        [TestCaseSource(typeof(Cases), "RenderUpdateTestCases")]
        public string RenderUpdate(UpdateQuery updateQuery, DatabaseType databaseType)
        {
            ISqlRenderer renderer;
            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.SqlServerCompact:
                    renderer = new SqlServerRenderer();
                    break;
                case DatabaseType.Mysql:
                case DatabaseType.Postgres:
                    renderer = new PostgresqlRenderer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("databaseType");
            }


            return renderer.Render(updateQuery);
        }
    }
}
