using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    public class InsertedValues : IEnumerable<InsertedValue>, IInsertable
    {
        private IReadOnlyCollection<InsertedValue> Columns => new ReadOnlyCollection<InsertedValue>(_columns);

        private readonly IList<InsertedValue> _columns;

        public InsertedValues()
        {
            _columns = new List<InsertedValue>();
        }

        public void Add(InsertedValue column)
        {
            _columns.Add(column);
        }

        public IEnumerator<InsertedValue> GetEnumerator() => Columns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Columns).GetEnumerator();
    }
}