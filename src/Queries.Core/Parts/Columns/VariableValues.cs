using Queries.Core.Parts.Clauses;
using System.Linq;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// A list of values to use in <see cref="Clauses.ClauseOperator.In"/> condition
    /// </summary>
    public class VariableValues : MultipleValues<Variable>
    {
        /// <summary>
        /// Builds a new <see cref="VariableValues"/> instance.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="others"></param>
        public VariableValues(Variable first, params Variable[] others) : base(first, others)
        {
        }

        /// <summary>
        /// Performs a deep copy of the current instance
        /// </summary>
        /// <returns></returns>
        public override IColumn Clone() => new VariableValues(Values.First(), Values.Skip(1).ToArray());

        public override bool Equals(ColumnBase other) => Equals(other as VariableValues);


        public override string ToString() => $"[{string.Join(",", Values)}]";
    }
}
