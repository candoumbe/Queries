using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Xunit;
using static Queries.Core.Parts.Clauses.ClauseLogic;

namespace Queries.Core.Tests.Parts
{
    public class CompositeWhereClauseTests
    {

        [Fact]
        public void DefaultCtor()
        {
            // Act
            CompositeWhereClause clause = new CompositeWhereClause();

            // Assert
            clause.Clauses.Should().BeEmpty();
            clause.Logic.Should().Be(And);
        }
    }
}
