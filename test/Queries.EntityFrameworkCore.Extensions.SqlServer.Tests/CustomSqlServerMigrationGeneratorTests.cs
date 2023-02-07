using Microsoft.EntityFrameworkCore.Migrations;
#if NET6_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Metadata;

#endif
using static Moq.MockBehavior;

using Moq;

using Queries.Renderers.SqlServer;
#if NET7_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Update; 
#endif

namespace Queries.EntityFrameworkCore.Extensions.SqlServer.Tests;

public class CustomSqlServerMigrationGeneratorTests
{
    private readonly CustomSqlServerMigrationGenerator _sut;
    private readonly Mock<MigrationsSqlGeneratorDependencies> _dependenciesMock;
#if NET7_0_OR_GREATER
    private readonly Mock<ICommandBatchPreparer> _annotationProviderMock;
#else
    private readonly Mock<IRelationalAnnotationProvider> _annotationProviderMock;
#endif
    private readonly SqlServerRenderer _renderer;


    public CustomSqlServerMigrationGeneratorTests()
    {
        _dependenciesMock = new();
        _annotationProviderMock = new(Strict);
        _renderer = new SqlServerRenderer();
        _sut = new(null, _annotationProviderMock.Object);
    }

    // [Property(Arbitrary = new[] { typeof(QueryGenerators) })]
    // public void Generate_should_create_expected_commands(DeleteQuery deleteQuery)
    // {
    //     // Arrange
    //     DeleteMigrationOperation op = new(deleteQuery);

    //     IReadOnlyList<MigrationOperation> operations = new List<MigrationOperation>() { op }
    //         .AsReadOnly();

    //     // Act
    //     IReadOnlyList<MigrationCommand> commands = _sut.Generate(operations, null);

    //     // Assert
    //     string expected = new SqlServerRenderer().Render(deleteQuery);

    //     commands.Once(cmd => cmd.CommandText == expected).ToProperty();
    //}
}
