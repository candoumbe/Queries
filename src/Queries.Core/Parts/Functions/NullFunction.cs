using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// "ISNULL" function
    /// </summary>
    [Function]
    public class NullFunction : IAliasable<NullFunction>, IColumn, IEquatable<NullFunction>
    {
        /// <summary>
        /// Column onto which the function must be applied.
        /// </summary>
        public IColumn Column { get; }

        /// <summary>
        /// Value to use as replacement when <see cref="Column"/>'s value is <c>null</c>
        /// </summary>
        public IColumn DefaultValue { get; }

        public IEnumerable<IColumn> AdditionalDefaultValues { get; }

        /// <summary>
        /// Builds a new <see cref="NullFunction"/> instance.
        /// </summary>
        /// <param name="column">The column to apply the function onto.</param>
        /// <param name="defaultValue">The default value value to use if <paramref name="column"/>'s value is <c>null</c>.</param>
        /// <param name="additionalDefaultValues"></param>
        /// <exception cref="ArgumentNullException"> if either <paramref name="column"/> or <paramref name="defaultValue"/> is <c>null</c></exception>
        public NullFunction(IColumn column, IColumn defaultValue, params IColumn[] additionalDefaultValues)
        {
            Column = column ?? throw new ArgumentNullException(nameof(column));
            DefaultValue = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue));
            AdditionalDefaultValues = additionalDefaultValues ?? Enumerable.Empty<IColumn>();
        }

        private string _alias;

        public string Alias => _alias;

        /// <summary>
        /// Sets the alias of the result of the function
        /// </summary>
        /// <param name="alias">the new alias</param>
        /// <returns>The current instance</returns>
        public NullFunction As(string alias)
        {
            _alias = alias;

            return this;
        }

        public override bool Equals(object obj) => Equals(obj as NullFunction);

        public bool Equals(NullFunction other) => (Column, DefaultValue).Equals((other?.Column, other?.DefaultValue));

        public override int GetHashCode() => (Column, DefaultValue).GetHashCode();

        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="NullFunction"/></returns>
        public IColumn Clone() => new NullFunction(Column.Clone(), DefaultValue, AdditionalDefaultValues?.ToArray());

        public override string ToString()
        {
            IDictionary<string, object> props = new Dictionary<string, object>
            {
                ["Function"] = nameof(NullFunction),
                [nameof(Column)] = Column,
                [nameof(DefaultValue)] = DefaultValue,
                [nameof(AdditionalDefaultValues)] = AdditionalDefaultValues.Where(val => val != null)
            };

            return props.Stringify();
        }
    }
}