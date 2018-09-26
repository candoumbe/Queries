using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    public class TruncateQueryTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public TruncateQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorWithNullArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new TruncateQuery(null);

            // Assert
            action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void CtorWithEmptyOrWhiteSpaceArgumentThrowsArgumentOutOfRangeException(string tableName)
        {
            // Act
            Action action = () => new TruncateQuery(tableName);

            // Assert
            action.Should().Throw<ArgumentOutOfRangeException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { Truncate("SuperHero"), null, false, "comparing with a null instance" };
                yield return new object[] { Truncate("SuperHero"), Truncate("SuperHero"), true, "comparing two instances with same tableName" };
                yield return new object[] { Truncate("SuperHero"), Select(1.Literal()), false, "comparing two different types of query" };
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(TruncateQuery first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        [Fact]
        public void HasDataManipulationLanguageAttribute()
        {
            // Arrange
            TypeInfo typeInfo = typeof(TruncateQuery)
                .GetTypeInfo();

            // Act
            Attribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

            // Assert
            attr.Should()
                .NotBeNull();
        }
    }
}
