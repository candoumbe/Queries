using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Queries.Core.Parts.Columns;

namespace Queries.Core.Builders
{
    /// <summary>
    /// A collection of values to insert. This class is intended to be used in conjuction with <see cref="InsertIntoQuery"/>
    /// </summary>
    public class InsertedValues : IEnumerable<InsertedValue>, IInsertable
    {
        private IReadOnlyCollection<InsertedValue> Columns => new ReadOnlyCollection<InsertedValue>(_columns);

        private readonly IList<InsertedValue> _columns;

        /// <summary>
        /// Builds a new <see cref="InsertedValues"/> instance.
        /// </summary>
        public InsertedValues()
        {
            _columns = new List<InsertedValue>();
        }

        /// <summary>
        /// Add <paramref name="column"/>
        /// </summary>
        /// <param name="column"></param>
        public void Add(InsertedValue column)
        {
            _columns.Add(column);
        }

        public IEnumerator<InsertedValue> GetEnumerator() => Columns.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Columns).GetEnumerator();
    }
}