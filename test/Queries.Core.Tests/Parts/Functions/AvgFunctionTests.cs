using FluentAssertions;
using Newtonsoft.Json.Linq;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Newtonsoft.Json.JsonConvert;
using Queries.Core.Attributes;

namespace Queries.Core.Tests.Parts.Functions
{
    public class AvgFunctionFunctionTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public AvgFunctionFunctionTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorThrowsArgumentNullExceptionIfColumnParameterIsNull()
        {
            // Act
            Action action = () => new AvgFunction((IColumn) null);

            // Assert
            action.Should().Throw<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void HasFunctionAttribute()
        {
            // Arrange 
            TypeInfo lengthFunctionType = typeof(AvgFunction)
                .GetTypeInfo();

            // Act
            FunctionAttribute attr = lengthFunctionType.GetCustomAttribute<FunctionAttribute>();

            // Assert
            attr.Should()
                .NotBeNull($"{nameof(AvgFunction)} must be marked with {nameof(FunctionAttribute)}");
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { Avg("Age".Field()), null, false, $"comparing {nameof(AvgFunction)} with a null instance" };
                yield return new object[] { Avg("Age".Field()), Avg("Age".Field()), true, $"comparing two {nameof(AvgFunction)} instances with same column names" };

                {
                    AvgFunction function = Avg("Age".Field());
                    yield return new object[] { function, function, true, $"comparing {nameof(AvgFunction)} instance to itself" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(AvgFunction first, object second, bool expectedResult, string reason)
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
