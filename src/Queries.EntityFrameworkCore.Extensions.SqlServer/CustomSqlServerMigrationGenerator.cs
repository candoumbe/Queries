using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Renderers.SqlServer;

namespace Queries.EntityFrameworkCore.Extensions.SqlServer
{
    /// <summary>
    /// Custom <see cref="SqlServerMigrationsSqlGenerator"/> implementation to handle <see cref="IQuery"/> instances.
    /// </summary>
    public class CustomSqlServerMigrationGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly SqlServerRenderer _renderer;

#if NET5_0
        public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] IRelationalAnnotationProvider relationalAnnotations) : base(dependencies, relationalAnnotations)
#else
        public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] IMigrationsAnnotationProvider migrationsAnnotations) : base(dependencies, migrationsAnnotations)
#endif
        {
            _renderer = new SqlServerRenderer();
        }

        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {

            if (operation is CreateViewMigrationOperation createViewMigrationOperation)
            {
                if (createViewMigrationOperation.Schema is not null)
                {
                    Generate(new EnsureSchemaOperation { Name = createViewMigrationOperation.Schema }, model, builder);
                }
                Generate(createViewMigrationOperation, builder);
            }
            else
            {
                base.Generate(operation, model, builder);
            }
        }

        private void Generate(in CreateViewMigrationOperation operation, in MigrationCommandListBuilder builder)
        {
            builder.AppendLine(_renderer.Render(operation.Query));
        }
    }
}
