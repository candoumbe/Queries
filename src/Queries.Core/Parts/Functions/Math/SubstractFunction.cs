using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Parts.Functions.Math
{
    /// <summary>
    /// Substract two or more columns.
    /// </summary>
    [Function]
    [JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
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
        public SubstractFunction(IColumn left, IColumn right)
        {
            Left = left;
            Right = right;
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

        public override bool Equals(object obj) => Equals(obj as SubstractFunction);

        public bool Equals(SubstractFunction other) => other != null 
            && Alias == other.Alias
            && Left.Equals(other.Left)
            && Right.Equals(other.Right);

        public override int GetHashCode()
        {
            int hashCode = -1367283405;
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Right);
            hashCode = hashCode * -1521134295 + EqualityComparer<IColumn>.Default.GetHashCode(Left);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alias);
            return hashCode;
        }

        public override string ToString() => SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// Performs a deep copy of the current instance.
        /// </summary>
        /// <returns><see cref="SubstractFunction"/></returns>
        public IColumn Clone() => new SubstractFunction(Left.Clone(), Right.Clone());

    }
}