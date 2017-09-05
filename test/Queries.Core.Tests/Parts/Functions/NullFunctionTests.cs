using FluentAssertions;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace Queries.Core.Tests.Parts.Functions
{
    /// <summary>
    /// Unit tests for <see cref="NullFunction"/>
    /// </summary>
    public class NullFunctionTests
    {
        public static IEnumerable<object[]> CtorThrowsArgumentNullExceptionCases
        {
            get
            {
                yield return new object[]{null, null};
                yield return new object[]{null, "".Literal()};
                yield return new object[]{"firstname".Field(), null};
            }
        }


        [Theory]
        [MemberData(nameof(CtorThrowsArgumentNullExceptionCases))]
        public void CtorThrowsArgumentNullExceptionIfAnyParameterIsNull(IColumn column, IColumn defaultValue)
        {
            // Act
            Action action = () => new NullFunction(column, defaultValue);

            // Assert
            action.ShouldThrow<ArgumentNullException>().Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }


        public static IEnumerable<object[]> CtorBuildAValidInstanceCases
        {
            get
            {
                yield return new object[] { "firstname".Field(), string.Empty.Literal() };
            }
        }

        [Theory]
        [MemberData(nameof(CtorBuildAValidInstanceCases))]
        public void CtorBuildAValidInstance(IColumn column, IColumn defaultValue)
        {
            // Act
            NullFunction function = new NullFunction(column, defaultValue);

            // Assert
            function.Column.Should().Be(column);
            function.DefaultValue.Should().Be(defaultValue);


        }

        [Fact]
        public void IsMarkedWithFunctionAttribute()
        {
            // Arrange
            TypeInfo typeInfo = typeof(NullFunction).GetTypeInfo();

            // Act
            IEnumerable<CustomAttributeData> customAttributes = typeInfo.CustomAttributes;


            // Arrange
            customAttributes.Should()
                .ContainSingle(attr => attr.AttributeType == typeof(FunctionAttribute));
        }
    }
}
