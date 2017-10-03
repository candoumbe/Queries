using System.Linq;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// A list of <see cref="string"/> values to use in <see cref="Clauses.ClauseOperator.In"/> condition
    /// </summary>
    public class StringValues : MultipleValues<string>
    {
        /// <summary>
        /// Builds a new <see cref="StringValues"/> instance.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="others"></param>
        public StringValues(string first, params string[] others) : base(first, others)
        {
        }

        /// <summary>
        /// Performs a deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override IColumn Clone() => new StringValues(Values.First(), Values.Skip(1).ToArray());

        public override string ToString() => $"[{string.Join(",", Values)}]";
        
    }
}
