using FluentAssertions;

using FsCheck;
using FsCheck.Xunit;

using Queries.Core.Renderers;

using System;

using Xunit.Categories;

namespace Queries.Core.Tests.Renderers
{
    [UnitTest]
    public class QueryWriterTests
    {
        [Property]
        public void Given_negative_blockLevel_ctor_should_throw_ArgumentOutOfRangeException(NegativeInt blockLevel)
        {
            // Act
            Action ctorWithNegativeBlockLevel = () => { QueryWriter writer = new(blockLevel.Item); };

            // Assert
            ctorWithNegativeBlockLevel.Should()
                                      .Throw<ArgumentOutOfRangeException>();
        }

        [Property]
        public Property Writer_should_works_consistenly(NonNegativeInt blockLevel, bool prettyPrint)
            => new QueryWriterSpecification(blockLevel.Item, prettyPrint).ToProperty();
    }
}
