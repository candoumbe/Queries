using FsCheck.Xunit;
using FsCheck;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Tests.Parts.Columns
{
    public class UniqueIdentifierValueTests
    {
        [Property]
        public Property Two_instances_are_equal(UniqueIdentifierValue first, UniqueIdentifierValue other) => (first.Equals(other)).Label("Equals implementation");
    }
}