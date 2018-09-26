using FluentAssertions;
using Queries.Core.Attributes;
using Queries.Core.Builders;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;

namespace Queries.Core.Tests.Builders
{
    public class DeleteQueryTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public DeleteQueryTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorWithNullArgumentThrowsArgumentNullException()
        {
            // Act
            Action action = () => new DeleteQuery(null);

            // Assert
            action.Should().Throw<ArgumentNullException>("name of the table to delete cannot be null").Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void HasDataManipulationLanguageAttribute()
        {
            // Arrange
            TypeInfo typeInfo = typeof(DeleteQuery).GetTypeInfo();

            // Act
            DataManipulationLanguageAttribute attr = typeInfo.GetCustomAttribute<DataManipulationLanguageAttribute>();

            // Arrange
            attr.Should()
                .NotBeNull($"{nameof(DeleteQuery)} must be marked with {nameof(DataManipulationLanguageAttribute)}");
        }


        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { Delete("firstname"), null, false, $"{nameof(DeleteQuery)} can never equals null" };
                yield return new object[] {
                    Delete("SuperHero"),
                    Delete("SuperHero"),
                    true,
                    $"Two {nameof(DeleteQuery)} instances with exactly the same {nameof(DeleteQuery.Table)} and no criteria must be equal." };

                yield return new object[] {
                    Delete("SuperHero"),
                    new SelectColumn(),
                    false,
                    $"{nameof(DeleteQuery)} can never equals an instance of an object that is not its type"};
                {
                    DeleteQuery query = Delete("SuperHero");
                    yield return new object[] { query, query, true, "Equals with same instance" };
                }

            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(DeleteQuery first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

    }
}
