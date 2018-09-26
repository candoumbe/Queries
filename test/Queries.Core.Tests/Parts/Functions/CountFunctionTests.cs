using System;
using System.Collections.Generic;
using Queries.Core.Extensions;
using Xunit;
using Queries.Core.Parts.Functions;
using FluentAssertions;
using System.Reflection;
using Queries.Core.Attributes;

namespace Queries.Core.Tests.Parts.Columns
{
    public class CountFunctionTests
    {
        [Fact]
        public void ConstructorTestWithNullArgument()
        {
            // Act
            Action action = () => new CountFunction(null);

            // Assert
            action.Should().Throw<ArgumentNullException>($"{nameof(CountFunction)} constructor called with null argument").Which
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


        [Fact]
        public void HasFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(CountFunction)
                .GetTypeInfo();

            // Act
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(CountFunction)} must be marked with {nameof(FunctionAttribute)}");
        }
    }
}