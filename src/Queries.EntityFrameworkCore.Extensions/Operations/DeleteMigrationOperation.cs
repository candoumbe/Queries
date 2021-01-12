using System;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Queries.Core.Builders;

namespace Queries.EntityFrameworkCore.Extensions.Operations
{
    public class DeleteMigrationOperation : DeleteDataOperation
    {
        public DeleteQuery Query { get; }

        public DeleteMigrationOperation(DeleteQuery query, string schema = null)
        {
            Schema = schema;
            Query = query ?? throw new ArgumentNullException(nameof(query));
            IsDestructiveChange = true;
        }
    }
}
