using FluentAssertions;
using Newtonsoft.Json.Linq;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions.Math;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Newtonsoft.Json.JsonConvert;
using Queries.Core.Attributes;

namespace Queries.Core.Tests.Parts.Functions.Math
{
    public class SubstractFunctionTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public SubstractFunctionTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        public static IEnumerable<object[]> CtorWithNullAsFirstOrSecondArgumentCases
        {
            get
            {
                yield return new object[] { null, null };
                yield return new object[] { 1.Literal(), null };
                yield return new object[] { null, 1.Literal() };
            }
        }

        [Theory]
        [MemberData(nameof(CtorWithNullAsFirstOrSecondArgumentCases))]
        public void CtorThrowsArgumentNullExceptionIfAnyParameterIsNull(IColumn first, IColumn second)
        {

            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");
            
            // Act
            Action action = () => new SubstractFunction(first, second);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }

        [Fact]
        public void HasFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(SubstractFunction)
                .GetTypeInfo();

            // Act
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(SubstractFunction)} must be marked with {nameof(FunctionAttribute)}");
        }


        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { "Left".Field().Substract("Right".Field()), null, false, "comparing with a null instance" };
                yield return new object[] { "Left".Field().Substract("Right".Field()), "Left".Field().Substract("Right".Field()), true, "comparing two instances with same columns names and same columns count" };
                yield return new object[] { "Left".Field().Substract("Right".Field()), new SubstractFunction("Left".Field(), "Right".Field()), true, "comparing two instances with same columns names and same columns count" };
                yield return new object[] { "Left".Field().Substract("Right".Field()), "Right".Field().Substract("Left".Field()), false, "comparing two instances with same columns names but in reversed order" };
                                                                    
                {                                                   
                    SubstractFunction function = "Left".Field().Substract("Right".Field());
                    yield return new object[] { function, function, true, "comparing instance to itself" };
                }
            }
        }


        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(SubstractFunction first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should().Be(expectedResult, reason);
        }

        public static IEnumerable<object[]> AsTestCases
        {
            get
            {
                yield return new object[]
                {
                    new SubstractFunction("left".Literal(), "right".Literal()).As(null),
                    null,
                };

                yield return new object[]
                {
                    new SubstractFunction("left".Literal(), "right".Literal()).As(null),
                    string.Empty,
                };
            }
        }

        [Theory]
        [MemberData(nameof(AsTestCases))]
        public void SettingAliasTest(SubstractFunction column, string expectedAlias)
            => column.Alias.Should().Be(expectedAlias);

    }
}
