using System;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Queries.Core.Builders;
using Xunit;
using Xunit.Categories;

namespace Queries.Core.Tests.Builders;

[UnitTest]
public class NativeQueryTests
{
    [Property]
    public Property Two_instances_with_same_statements_are_equal(NonEmptyString statement)
    {
        // Act
        NativeQuery first = new(statement.Item);
        NativeQuery other = new(statement.Item);

        return first.Equals(other).And(first.GetHashCode() == other.GetHashCode());
    }

    [Fact]
    public void Ctor_throws_ArgumentNullException_when_Statement_is_null()
    {
        // Act
        Action ctorWithNullStatement = () => new NativeQuery(null);

        // Assert
        ctorWithNullStatement.Should()
                             .ThrowExactly<ArgumentNullException>();
    }
}