using System;
using System.Collections.Generic;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using Xunit.Abstractions;
using static Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json.Linq;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(FieldColumn))]
    public class FieldColumnTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public FieldColumnTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            // Act
            Action action = () => new FieldColumn(null);

            // Arrange
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyArgument()
        {
            // Act
            Action action = () => new FieldColumn(string.Empty);

            // Arrange
            action.Should().Throw<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithWhitespaceStringArgument()
        {
            // Act
            Action action = () => new FieldColumn("   ");

            // Arrange
            action.Should().Throw<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new CountFunction("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new CountFunction("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(CountFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new FieldColumn("firstname"), null, false, "object is null" };
                yield return new object[] { new FieldColumn("firstname"), new FieldColumn("firstname"), true, $"object is a {nameof(FieldColumn)} with exactly the same {nameof(FieldColumn.Name)} and {nameof(FieldColumn.Alias)}" };
                yield return new object[] { new FieldColumn("firstname"), new SelectColumn(), false, $"{nameof(FieldColumn)} is always != exactly the same {nameof(SelectColumn)}" };

                {
                    FieldColumn column = new FieldColumn("firstname");
                    yield return new object[] { column, column, true, "Equals with same instance" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(FieldColumn first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"First : {first}");
            _outputHelper.WriteLine($"Second : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        public static IEnumerable<object[]> ToStringCases
        {
            get
            {
                yield return new object[] { new FieldColumn("Firstname"), new JObject{ ["Name"] = "Firstname", ["Alias"] = null }.ToString(Newtonsoft.Json.Formatting.None)};
                yield return new object[] { new FieldColumn("Firstname").As("First name"), SerializeObject(new { Name = "Firstname", Alias = "First name" })};
            }
        }

        [Theory]
        [MemberData(nameof(ToStringCases))]
        public void TestsToString(FieldColumn column, string expected)
        {
            // Act
            string actual = column.ToString();

            // Assert
            actual.Should().Be(expected);
        }

        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[] { "Firstname".Field() };
            }
        }
        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(FieldColumn original)
        {
            // Act
            FieldColumn copie = (FieldColumn) original.Clone();

            // Assert
            copie.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}