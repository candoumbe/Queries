using System;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Queries.Core.Builders;

namespace Queries.EntityFrameworkCore.Extensions.Operations
{
    /// <summary>
    ///  <see cref="MigrationOperation"/> to create a SQL view
    /// </summary>
    public class CreateViewMigrationOperation : MigrationOperation
    {
        public CreateViewQuery Query { get; }
        public string Schema { get; }

        public CreateViewMigrationOperation(CreateViewQuery query, string schema = null)
        {
            Schema = schema;
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }
    }
}
