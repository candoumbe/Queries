using FluentAssertions;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts
{
    [UnitTest]
    [Feature("Variable")]
    public class VariableTests :IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public VariableTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose() => _outputHelper = null;

        public static IEnumerable<object[]> CloneCases
        {
            get
            {
                yield return new[]
                {
                    new Variable("p", VariableType.String, "value")
                };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(Variable original)
        {
            // Act
            IColumn copy = original.Clone();

            // Assert
            copy.Should()
                .BeOfType<Variable>().And
                .NotBeSameAs(original, $"original.{nameof(Variable.Clone)}() == original)should always be false").And
                .Be(original, $"original.{nameof(Variable.Clone)}().Equals(original) should always be true");
        }


        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new Variable("firstname", VariableType.String, "Clark"), null, false, "comparing with a null instance" };
                yield return new object[] { new Variable("firstname", VariableType.String, "Clark"), new Variable("firstname", VariableType.String, "Clark"), true, "comparing two instances with same name/type/value" };
                yield return new object[] { new Variable("firstname", VariableType.Date, "Clark"), new Variable("firstname", VariableType.String, "Clark"), false, "comparing two instances with same name/value but different types." };
                {
                    Variable variable = new Variable("firstname", VariableType.Date, "Clark");
                    yield return new object[] { variable, variable.Clone(), true, "comparing a variable to its clone." };
                }
            }
        }


        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(Variable first, object second, bool expectedResult, string reason)
        {
            _outputHelper.WriteLine($"{nameof(first)} : {first}");
            _outputHelper.WriteLine($"{nameof(second)} : {second}");

            // Act
            bool actualResult = first.Equals(second);

            // Assert
            actualResult.Should()
                .Be(expectedResult, reason);
        }
    }
}
