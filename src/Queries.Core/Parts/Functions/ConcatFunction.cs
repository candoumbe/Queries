using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Queries.Core.Attributes;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Parts.Functions
{
    /// <summary>
    /// Concat two or more columns.
    /// </summary>
    [Function]
    [JsonObject(ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
    public class ConcatFunction : IAliasable<ConcatFunction>, IColumn, IEquatable<ConcatFunction>
    {
        /// <summary>
        /// List of columns to concat
        /// </summary>
        public IEnumerable<IColumn> Columns { get; }

        /// <summary>
        /// Builds a new <see cref="ConcatFunction"/> instance.
        /// </summary>
        /// <param name="first">First column to concatenate</param>
        /// <param name="second">Second column to concatenate</param>
        /// <param name="columns">Additional columns to concatenate.</param>
        /// <exception cref="ArgumentNullException">if either <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.</exception>
        public ConcatFunction(IColumn first, IColumn second, params IColumn[] columns)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }

            IEnumerable<IColumn> localColumns = (columns ?? Enumerable.Empty<IColumn>())
                .Where(x => x != null)
                .ToList();
            
            Columns = new[] { first, second }.Union(localColumns);
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
        public ConcatFunction As(string alias)
        {
            _alias = alias;

            return this;
        }

        public override bool Equals(object obj) => Equals(obj as ConcatFunction);

        public bool Equals(ConcatFunction other) => other != null 
            && Columns.SequenceEqual(other.Columns) && Alias == other.Alias;

        public override int GetHashCode()
        {
            int hashCode = -1367283405;
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<IColumn>>.Default.GetHashCode(Columns);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Alias);
            return hashCode;
        }

        public override string ToString()
        {
            JObject jObj = new JObject
            {
                ["Type"] = nameof(ConcatFunction),
                [nameof(Columns)] = JToken.FromObject(Columns),
                [nameof(Alias)] = Alias
            };
            return jObj.ToString();
        }
    }
}