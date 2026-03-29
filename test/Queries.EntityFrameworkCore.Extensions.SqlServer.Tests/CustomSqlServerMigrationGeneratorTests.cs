using Microsoft.EntityFrameworkCore.Migrations;
#if NET6_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Metadata;

#endif
using NSubstitute;

using Queries.Renderers.SqlServer;
#if NET7_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Update;
using FsCheck.Xunit;
using TestsHelpers;
using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Operations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using FsCheck.Fluent;
#endif

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;


namespace Queries.EntityFrameworkCore.Extensions.SqlServer.Tests;

public class CustomSqlServerMigrationGeneratorTests
{
    private readonly CustomSqlServerMigrationGenerator _sut;
    private readonly MigrationsSqlGeneratorDependencies _dependenciesMock;
#if NET7_0_OR_GREATER
    private readonly ICommandBatchPreparer _annotationProviderMock;
#else
    private readonly IRelationalAnnotationProvider _annotationProviderMock;
#endif

    private readonly SqlServerRenderer _renderer = new();

    public CustomSqlServerMigrationGeneratorTests()
    {
        IRelationalCommandBuilder commandBuilder = Substitute.For<IRelationalCommandBuilder>();
        commandBuilder.Append(Arg.Any<string>()).Returns(commandBuilder);

        IRelationalCommandBuilderFactory commandBuilderFactory = Substitute.For<IRelationalCommandBuilderFactory>();
        commandBuilderFactory.Create().Returns(commandBuilder);

        ISqlGenerationHelper sqlGenerationHelper = Substitute.For<ISqlGenerationHelper>();
        sqlGenerationHelper.StatementTerminator.Returns(";");
        sqlGenerationHelper.BatchTerminator.Returns(string.Empty);

        ILoggingOptions loggingOptions = Substitute.For<ILoggingOptions>();
        loggingOptions.IsSensitiveDataLoggingEnabled.Returns(false);

#pragma warning disable EF1001 // Internal EF Core API usage.
        _dependenciesMock = new(
            commandBuilderFactory,
            Substitute.For<IUpdateSqlGenerator>(),
            sqlGenerationHelper,
            Substitute.For<IRelationalTypeMappingSource>(),
            Substitute.For<ICurrentDbContext>(),
            Substitute.For<IModificationCommandFactory>(),
            loggingOptions,
            Substitute.For<IRelationalCommandDiagnosticsLogger>(),
            Substitute.For<IDiagnosticsLogger<DbLoggerCategory.Migrations>>());
#pragma warning restore EF1001 // Internal EF Core API usage.

#if NET7_0_OR_GREATER
        _annotationProviderMock = Substitute.For<ICommandBatchPreparer>();
#else
        _annotationProviderMock = Substitute.For<IRelationalAnnotationProvider>();
#endif
        _sut = new(_dependenciesMock, _annotationProviderMock);
    }

    [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
    public void Generate_should_create_expected_commands(DeleteQuery deleteQuery)
    {
        // Arrange
        DeleteMigrationOperation op = new(deleteQuery);

        IReadOnlyList<MigrationOperation> operations = new List<MigrationOperation>() { op }
            .AsReadOnly();

        // Act
        IReadOnlyList<MigrationCommand> commands = _sut.Generate(operations, null);

        // Assert
        string expected = _renderer.Render(deleteQuery);

        commands.Once(cmd => cmd.CommandText == expected).ToProperty();
    }
}
