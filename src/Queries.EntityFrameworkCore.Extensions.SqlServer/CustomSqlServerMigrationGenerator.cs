using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
#if NET7_0_OR_GREATER
using Microsoft.EntityFrameworkCore.Update; 
#endif

using Queries.Core;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Renderers.SqlServer;

using System.Diagnostics.CodeAnalysis;

namespace Queries.EntityFrameworkCore.Extensions.SqlServer;

/// <summary>
/// Custom <see cref="SqlServerMigrationsSqlGenerator"/> implementation to handle <see cref="IQuery"/> instances.
/// </summary>
public class CustomSqlServerMigrationGenerator : SqlServerMigrationsSqlGenerator
{
    private readonly SqlServerRenderer _renderer;

    ///<inheritdoc/>
#if NET7_0_OR_GREATER
    public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] ICommandBatchPreparer commandBatchPreparer) : base(dependencies, commandBatchPreparer)
#else
    public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] IRelationalAnnotationProvider relationalAnnotations) : base(dependencies, relationalAnnotations)
#endif
    {
        _renderer = new();
    }

    /// <inheritdoc />
    protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
    {
        switch (operation)
        {
            case CreateViewMigrationOperation createViewMigrationOperation:
                if (createViewMigrationOperation.Schema is not null)
                {
                    Generate(new EnsureSchemaOperation { Name = createViewMigrationOperation.Schema }, model, builder);
                }
                Generate(createViewMigrationOperation.Query, builder);
                break;
            case DeleteMigrationOperation deleteMigrationOperation:
                if (deleteMigrationOperation.Schema is not null)
                {
                    Generate(new EnsureSchemaOperation { Name = deleteMigrationOperation.Schema }, model, builder);
                }
                Generate(deleteMigrationOperation.Query, builder);
                break;
            default:
                base.Generate(operation, model, builder);
                break;
        }
    }

    private void Generate(in IQuery query, in MigrationCommandListBuilder builder)
        => builder.AppendLine(_renderer.Render(query));
}
