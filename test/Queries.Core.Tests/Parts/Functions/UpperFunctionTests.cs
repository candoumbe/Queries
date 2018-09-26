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
    public class UpperFunctionTests
    {

        [Fact]
        public void ConstructorTestWithNullStringArgument()
        {
            Action action = () => new UpperFunction((string)null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        
        [Fact]
        public void ConstructorTestWithNullColumnArgument()
        {
            Action action = () => new UpperFunction((IColumn) null);

            action.Should().ThrowExactly<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }
        
        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(UpperFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);



        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new UpperFunction("firstname".Field()),
                    null,
                };

                yield return new object[]
                {
                    new UpperFunction("firstname".Field()).As(string.Empty),
                    string.Empty,
                };
            }
        }

        [Fact]
        public void HasFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(UpperFunction)
                .GetTypeInfo();

            // Act
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(UpperFunction)} must be marked with {nameof(FunctionAttribute)}");
        }
    }
}