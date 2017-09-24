using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// Represents a UUID value
    /// </summary>
    public sealed class UniqueIdentifierValue : ColumnBase, IColumn
    {
        public override IColumn Clone() => new UniqueIdentifierValue();
    }
}
