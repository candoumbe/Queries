using FluentAssertions;
using Queries.Core.Parts.Columns;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Queries.Core.Tests.Parts.Columns
{
    public class StringValuesTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public StringValuesTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new StringValues("one", "two", "three"), null, false, "comparing with a null instance" };
                yield return new object[] { new StringValues("one", "two", "three"), new StringValues("one", "two", "three"), true, "comparing two instances with same values of same type" };
                yield return new object[] { new StringValues("one", "two", "three"), new StringValues("one", "three", "two"), false, "comparing two instances with of same values but in different order." };
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(StringValues first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[] { new StringValues("Alice", "Bob", "Charles") };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(StringValues original)
        {
            _outputHelper.WriteLine($"{nameof(original)} : {original}");

            // Act
            IColumn copie = original.Clone();

            // Assert
            copie.Should()
                .BeOfType<StringValues>().Which.Should()
                .NotBeSameAs(original).And
                .BeEquivalentTo(original);
        }
    }
}
