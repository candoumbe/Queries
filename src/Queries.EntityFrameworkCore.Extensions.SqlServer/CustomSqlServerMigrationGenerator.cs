using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Queries.EntityFrameworkCore.Extensions.Operations;
using Queries.Renderers.SqlServer;
using Queries.Core;

namespace Queries.EntityFrameworkCore.Extensions.SqlServer
{
    /// <summary>
    /// Custom <see cref="SqlServerMigrationsSqlGenerator"/> implementation to handle <see cref="IQuery"/> instances.
    /// </summary>
    public class CustomSqlServerMigrationGenerator : SqlServerMigrationsSqlGenerator
    {
        private readonly SqlServerRenderer _renderer;

        ///<inheritdoc/>
#if NET5_0_OR_GREATER
        public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] IRelationalAnnotationProvider relationalAnnotations) : base(dependencies, relationalAnnotations)
#else
        public CustomSqlServerMigrationGenerator([NotNull] MigrationsSqlGeneratorDependencies dependencies, [NotNull] IMigrationsAnnotationProvider migrationsAnnotations) : base(dependencies, migrationsAnnotations)
#endif
        {
            _renderer = new SqlServerRenderer();
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
}
