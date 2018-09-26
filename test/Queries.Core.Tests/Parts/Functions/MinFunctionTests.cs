using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using System.Reflection;
using Queries.Core.Attributes;

namespace Queries.Core.Tests.Parts.Columns
{
    public class MinFunctionTests
    {
        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new MinFunction((string)null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithEmptyStringArgument()
        {
            Action action = () => new MinFunction(string.Empty);

            action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public void ConstructorTestWithWhitespaceStringArgument(string columnName)
        {
            Action action = () => new MinFunction(columnName);

            action.Should().ThrowExactly<ArgumentOutOfRangeException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new MinFunction((IColumn) null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void ConstructorTestColumnArgument() => 
            new MinFunction("age").Type.Should().Be(AggregateType.Min);
        
        
        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new MinFunction("age".Field()),
                    null,
                };

                yield return new object[]
                {
                    new MinFunction("age".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(MinFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

        [Fact]
        public void HasFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(MinFunction)
                .GetTypeInfo();

            // Act
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(MinFunction)} must be marked with {nameof(FunctionAttribute)}");
        }
    }
}