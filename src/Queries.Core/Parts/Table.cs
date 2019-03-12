using System;
using System.Collections.Generic;

namespace Queries.Core.Parts
{
    public class Table : INamable, ITable, IAliasable<Table>, IEquatable<Table>
    {
        /// <summary>
        /// Gets the name of the table in a query
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get;}

        /// <summary>
        /// Builds a new <see cref="Table"/> instance.
        /// </summary>
        /// <param name="tablename">Name of the table</param>
        /// <param name="alias">alias of the table</param>
        public Table(string tablename, string alias = null)
        {
            Name = tablename ?? throw new ArgumentNullException(nameof(tablename));
            Alias = alias;
        }

        /// <summary>
        /// Gets the alias of the table
        /// </summary>
        public string Alias { get; private set; }

        public Table As(string alias)
        {
            Alias = alias;
            return this;
        }

        public override bool Equals(object obj) => Equals(obj as Table);
        public bool Equals(Table other) => (Name, Alias).Equals((other?.Name, other?.Alias));

        public override int GetHashCode() => (Name, Alias).GetHashCode();

        public ITable Clone() => new Table(Name, Alias);
    }
}
