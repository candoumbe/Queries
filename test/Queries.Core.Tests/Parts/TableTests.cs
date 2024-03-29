﻿using FluentAssertions;

using Queries.Core.Parts;

using System;
using System.Collections.Generic;

using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts;

/// <summary>
/// Unit tests for <see cref="Table"/>
/// </summary>
[UnitTest]
[Feature(nameof(Table))]
public class TableTests : IDisposable
{
    private ITestOutputHelper _outputHelper;

    public TableTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

    public void Dispose() => _outputHelper = null;

    [Fact]
    public void CtorThrowsArgumentNullExceptionWhenParameterIsNull()
    {
        // Act
        Action action = () => new Table(null);

        // Assert
        action.Should().Throw<ArgumentNullException>().Which
            .ParamName.Should()
            .NotBeNullOrWhiteSpace();
    }

    public static IEnumerable<object[]> EqualsCases
    {
        get
        {
            yield return new object[] { new Table("firstname"), null, false, "object is null" };
            yield return new object[] { new Table("firstname"), new Table("firstname"), true, $"object is a {nameof(Table)} with exactly the same {nameof(Table.Name)} and {nameof(Table.Alias)}" };
            
            {
                Table column = new("firstname");
                yield return new object[] { column, column, true, "Equals with same instance" };
            }
        }
    }

    [Theory]
    [MemberData(nameof(EqualsCases))]
    public void EqualTests(Table first, object second, bool expectedResult, string reason)
    {
        _outputHelper.WriteLine($"First : {first}");
        _outputHelper.WriteLine($"Second : {second}");

        // Act
        bool actualResult = first.Equals(second);

        // Assert
        actualResult.Should().Be(expectedResult, reason);
    }
}
