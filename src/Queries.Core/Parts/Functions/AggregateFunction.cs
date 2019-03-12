using Newtonsoft.Json;
using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Base class for all aggregate function.
    /// </summary>
    [Function]
    [JsonObject]
    public abstract class AggregateFunction : IAliasable<AggregateFunction>, IEquatable<AggregateFunction>, IColumn
    {
        /// <summary>
        /// The type of aggregate the current instance represents
        /// </summary>
        public AggregateType Type { get; }

        /// <summary>
        /// The column onto which the current function will be applied
        /// </summary>
        public IColumn Column { get; }

        /// <summary>
        /// Builds a new <see cref="AggregateFunction"/> instance.
        /// </summary>
        /// <param name="aggregate">The aggregate function to create</param>
        /// <param name="column">The cokumn onto which the aggregate will be applied</param>
        /// <exception cref="ArgumentNullException">if <paramref name="column"/> is null.</exception>
        protected AggregateFunction(AggregateType aggregate, IColumn column)
        {
            Type = aggregate;
            Column = column ?? throw new ArgumentNullException(nameof(column));
        }

        private string _alias;

        /// <summary>
        /// The alias associated with the column
        /// </summary>
        public string Alias => _alias;

        public AggregateFunction As(string alias)
        {
            _alias = alias;

            return this;
        }

        public override bool Equals(object obj) => Equals(obj as AggregateFunction);

        public bool Equals(AggregateFunction other) => (Column, Alias).Equals((other?.Column, other?.Alias));

        public override int GetHashCode() => (Column, Alias).GetHashCode();

        public override string ToString() => SerializeObject(this);

        public abstract IColumn Clone();
    }
}
