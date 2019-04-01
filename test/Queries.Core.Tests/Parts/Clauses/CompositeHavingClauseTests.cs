using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Xunit;
using Xunit.Categories;
using static Queries.Core.Parts.Clauses.ClauseLogic;

namespace Queries.Core.Tests.Parts
{
    [UnitTest]
    [Feature("Having clause")]
    public class CompositeHavingClauseTests
    {
        [Fact]
        public void DefaultCtor()
        {
            // Act
            CompositeHavingClause clause = new CompositeHavingClause();

            // Assert
            clause.Clauses.Should().BeEmpty();
            clause.Logic.Should().Be(And);
        }

        [Fact]
        public void IsAHavingClause()
            => new CompositeHavingClause().Should().BeAssignableTo<IHavingClause>();
    }
}
