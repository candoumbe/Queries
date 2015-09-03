using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Queries.Builders.Fluent;
using Queries.Parts.Columns;

namespace Queries.Tests.Renderers
{
    public class SelectQueryBuilderTest
    {

        private SelectQueryBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            
            _builder = new SelectQueryBuilder();
            
        }


        [TearDown]
        public void TearDown()
        {
            _builder = null;
        }



        private class Cases
        {
            public IEnumerable<ITestCaseData> SelectQueryBuilderTestCases
            {
                get
                {
                    yield return new TestCaseData(new SelectQueryBuilder()
                        .Select(new TableColumn(){ Name = }));
                }
            }
        }
    }
}
