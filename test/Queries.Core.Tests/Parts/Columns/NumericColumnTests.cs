using FluentAssertions;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Queries.Core.Tests.Parts.Columns
{
    [UnitTest]
    [Feature(nameof(NumericColumn))]
    [Feature("Column")]
    public class NumericColumnTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public NumericColumnTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose() => _outputHelper = null;

        [Fact]
        public void CtorShouldSetValueAsPassedIn()
        {
            new NumericColumn((int?)null).Value.Should().BeNull($"new {nameof(NumericColumn)}((int?)null).Value should be null");

            new NumericColumn((long?)null).Value.Should().BeNull($"new {nameof(NumericColumn)}((long?)null).Value should be null");

            new NumericColumn((float?)null).Value.Should().BeNull($"new {nameof(NumericColumn)}((float?)null).Value should be null");

            new NumericColumn((double?)null).Value.Should().BeNull($"new {nameof(NumericColumn)}((double?)null).Value should be null");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void CtorWithIntArgument(int value)
        {
            _outputHelper.WriteLine($"{nameof(value)} : {value}");

            // Act
            NumericColumn column = new NumericColumn(value);

            // Assert
            int? currentValue = column.Value.Should()
                .BeAssignableTo<int>().Which;

            currentValue.Should().Be(value, $"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(-1L)]
        [InlineData(long.MaxValue)]
        [InlineData(long.MinValue)]
        public void CtorWithLongArgument(long value)
        {
            _outputHelper.WriteLine($"{nameof(value)} : {value}");

            // Act
            NumericColumn column = new NumericColumn(value);

            // Assert
            column.Value.Should()
                .BeAssignableTo<long>().And
                .Be(value, $"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MinValue)]
        public void CtorWithDoubleArgument(double value)
        {
            _outputHelper.WriteLine($"{nameof(value)} : {value}");

            // Act
            NumericColumn column = new NumericColumn(value);

            // Assert
            column.Value.Should()
                .BeAssignableTo<double>().Which
                .Should().BeApproximately(value, double.Epsilon, $"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Theory]
        [InlineData(0F)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        [InlineData(float.NegativeInfinity)]
        [InlineData(float.PositiveInfinity)]
        [InlineData(float.Epsilon)]
        public void CtorWithFloatArgument(float value)
        {
            _outputHelper.WriteLine($"{nameof(value)} : {value}");

            // Act
            NumericColumn column = new NumericColumn(value);

            // Assert
            column.Value.Should()
                .BeAssignableTo<float>().Which
                .Should().Be(value, $"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Fact]
        public void CtorWithFloatNullArgument()
        {
            // Act
            NumericColumn column = new NumericColumn((float?) null);

            // Assert
            column.Value.Should()
                .BeNull($"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Fact]
        public void CtorWithDoubleNullArgument()
        {
            // Act
            NumericColumn column = new NumericColumn((double?)null);

            // Assert
            column.Value.Should()
                .BeNull($"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        [Fact]
        public void CtorWithLongNullArgument()
        {
            // Act
            NumericColumn column = new NumericColumn((long?)null);

            // Assert
            column.Value.Should()
                .BeNull($"{nameof(NumericColumn)}.{nameof(NumericColumn.Value)} should be equal to the ctor input");
        }

        public static IEnumerable<object[]> EqualsCases
        {
            get
            {
                yield return new object[] { new NumericColumn(1), null, false, "object is null" };
                yield return new object[] { new NumericColumn(1), new NumericColumn(1), true, $"object is a {nameof(NumericColumn)} with exactly the same {nameof(NumericColumn.Value)} and {nameof(NumericColumn.Alias)}" };
                yield return new object[] { new NumericColumn(1), new SelectColumn(), false, $"{nameof(NumericColumn)} is always != from {nameof(SelectColumn)}" };

                {
                    NumericColumn column = new NumericColumn(1);
                    yield return new object[] { column, column, true, "Equals with same instance" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(EqualsCases))]
        public void EqualTests(NumericColumn first, object second, bool expectedResult, string reason)
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
                yield return new[] { 1.Literal() };
                yield return new[] { 2.0f.Literal() };
            }
        }

        [Theory]
        [MemberData(nameof(CloneCases))]
        public void CloneTest(NumericColumn original)
        {
            _outputHelper.WriteLine($"{nameof(original)} : {original}");

            // Act
            IColumn copie = original.Clone();

            // Assert
            copie.Should()
                .BeOfType<Literal>().Which.Should()
                .NotBeSameAs(original).And
                .Be(original);
        }
    }
}
