using System;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// a <see cref="Variable"/> can be used in <see cref="WhereClause.Constraint"/>
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Builds a new <see cref="Variable"/> instance.
        /// </summary>
        /// <param name="name">name of the variable</param>
        /// <param name="type">type of variable</param>
        /// <param name="value">value of the variable</param>
        public Variable(string name, VariableType type, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
            Name = $"{name.Substring(0,1).ToLowerInvariant()}{name.Substring(1)}";
            Value = value;
            Type = type;
        }

       
        /// <summary>
        /// Name of the parameter
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Value of the parameter
        /// </summary>
        public object Value { get;  }

        /// <summary>
        /// Type of the constraint's value
        /// </summary>
        public VariableType Type { get; }
    }
}