using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

using Queries.Core.Builders;
using Queries.EntityFrameworkCore.Extensions.Operations;

namespace Queries.EntityFrameworkCore.Extensions;

public static class MigrationBuilderExtensions
{

    /// <summary>
    ///  Adds a <see cref="MigrationOperation"> to create a view.
    /// </summary>
    /// <param name="builder"></param>i        /// <param name="query"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static MigrationBuilder CreateView(this MigrationBuilder builder, CreateViewQuery query, string schema = null)
    {
        builder.Operations.Add(new CreateViewMigrationOperation(query, schema));
        return builder;
    }

    /// <summary>
    ///  Adds a <see cref="MigrationOperation"> to delete data.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="query"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    public static MigrationBuilder Delete(this MigrationBuilder builder, DeleteQuery query, string schema = null)
    {
        builder.Operations.Add(new DeleteMigrationOperation(query, schema));
        return builder;
    }
}
