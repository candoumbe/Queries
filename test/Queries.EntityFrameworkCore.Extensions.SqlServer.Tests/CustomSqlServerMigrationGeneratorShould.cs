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

public class CustomSqlServerMigrationGeneratorShould
{
    private readonly CustomSqlServerMigrationGenerator _sut;
    private readonly MigrationsSqlGeneratorDependencies _dependenciesMock;
#if NET7_0_OR_GREATER
    private readonly ICommandBatchPreparer _annotationProviderMock;
#else
    private readonly IRelationalAnnotationProvider _annotationProviderMock;
#endif

    private readonly SqlServerRenderer _renderer = new();

    public CustomSqlServerMigrationGeneratorShould()
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

    [Property(Arbitrary = [typeof(QueryGenerators) ])]
    public void Render_DeleteQuery_command_When_DeleteQuery_is_provided(DeleteQuery deleteQuery)
    {
        // Arrange
        string expected = _renderer.Render(deleteQuery);
        DeleteMigrationOperation op = new(deleteQuery);

        IReadOnlyList<MigrationOperation> operations = [op];

        // Act
        IReadOnlyList<MigrationCommand> commands = _sut.Generate(operations, null);

        // Assert
        commands.Once(cmd => cmd.CommandText == expected).ToProperty();
    }

    // insert tests for other query types (InsertQuery, UpdateQuery, etc.) here
    [Property(Arbitrary = [typeof(QueryGenerators) ])]
    public void Render_CreateView_command_When_CreateViewQuery_is_provided(CreateViewQuery createViewQuery)
    {
        // Arrange
        string expected = _renderer.Render(createViewQuery);
        CreateViewMigrationOperation op = new(createViewQuery);

        IReadOnlyList<MigrationOperation> operations = [ op ];

        // Act
        IReadOnlyList<MigrationCommand> commands = _sut.Generate(operations, null);

        // Assert
        commands.Once(cmd => cmd.CommandText == expected).ToProperty();
    }
}
