using FluentAssertions;
using Queries.Core.Extensions;
using Queries.Core.Parts.Columns;
using Queries.Core.Parts.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Queries.Core.Tests.Parts.Functions
{
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
    }
}
