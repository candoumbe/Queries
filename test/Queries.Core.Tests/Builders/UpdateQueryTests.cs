using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Parts;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    public class UpdateQueryTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public UpdateQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorWithNullStringArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new UpdateQuery((string) null);

            // Assert
            action.ShouldThrow<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CtorWithNullTableArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new UpdateQuery((Table)null);

            // Assert
            action.ShouldThrow<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void CtorWithEmptyOrWhiteSpaceArgumentThrowsArgumentOutOfRangeException(string tableName)
        {
            // Act
            Action action = () => new UpdateQuery(tableName);

            // Assert
            action.ShouldThrow<ArgumentOutOfRangeException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { Update("SuperHero").Set("firstname".Field().EqualTo("Bruce")), null, false, "comparing with a null instance" };
                yield return new object[] { Update("SuperHero").Set("firstname".Field().EqualTo("Bruce")), Update("SuperHero").Set("firstname".Field().EqualTo("Bruce")), true, "comparing two instances with same tableName" };
                yield return new object[] { Update("SuperHero").Set("firstname".Field().EqualTo("Bruce")), Update("SuperHero").Set("firstname".Field().EqualTo("Bruce")), true, "comparing two instances with same tableName" };
                yield return new object[] { Update("SuperHero"), Select(1.Literal()), false, "comparing two different types of query" };
            }
        }


        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(UpdateQuery first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        [Fact]
        public void IsDataManipulationQuery()
        {
            // Arrange
            TypeInfo typeInfo = typeof(UpdateQuery)
                .GetTypeInfo();

            // Act
            Attribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(UpdateQuery)} class must be marked with {nameof(DataManipulationLanguageAttribute)}");
        }



    }
}
