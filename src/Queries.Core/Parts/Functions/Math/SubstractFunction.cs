#if !SYSTEM_TEXT_JSON
using Newtonsoft.Json; 
#endif

using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;

namespace Queries.Core.Parts.Functions.Math
{
    /// <summary>
    /// Substract two or more columns.
    /// </summary>
    [Function]
#if !SYSTEM_TEXT_JSON
[JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
#endif
    public class SubstractFunction : IAliasable<SubstractFunction>, IColumn, IEquatable<SubstractFunction>
    {
        /// <summary>
        /// Left part of the operation
        /// </summary>
        public IColumn Left { get; }

        /// <summary>
        /// Right part of the operation
        /// </summary>
        public IColumn Right { get; set; }

        /// <summary>
        /// Builds a new <see cref="SubstractFunction"/> instance.
        /// </summary>
        /// <param name="left">First column of the operation</param>
        /// <param name="right">Second column of the operation</param>
        /// <exception cref="ArgumentNullException">if either <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.</exception>
        public SubstractFunction(IColumn left, IColumn right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        private string _alias;

        /// <summary>
        /// Alias of the result of the result of the function.
        /// </summary>
        public string Alias => _alias;

        /// <summary>
        /// Gives the result of the concat an alias
        /// </summary>
        /// <param name="alias">The new alias</param>
        /// <returns>The current instance</returns>
        public SubstractFunction As(string alias)
        {
            _alias = alias;

            return this;
        }

        ///<inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as SubstractFunction);

        ///<inheritdoc/>
        public bool Equals(SubstractFunction other) => other != null
            && Alias == other.Alias
            && Left.Equals(other.Left)
            && Right.Equals(other.Right);

        ///<inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -1367283405;
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Right);
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alias);
            return hashCode;
        }

        ///<inheritdoc/>
        public override string ToString() => this.Jsonify();

        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="SubstractFunction"/></returns>
        public IColumn Clone() => new SubstractFunction(Left.Clone(), Right.Clone());
    }
}