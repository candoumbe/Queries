using Newtonsoft.Json;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using static Newtonsoft.Json.JsonConvert;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// a <see cref="Variable"/> can be used in <see cref="WhereClause.Constraint"/>
    /// </summary>
    public class Variable : ColumnBase, IEquatable<Variable>
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
            Name = $"{name.Substring(0, 1).ToLowerInvariant()}{name.Substring(1)}";
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

        public override IColumn Clone() => new Variable(Name, Type, Value);

        public override bool Equals(object obj) => Equals(obj as Variable);

        public bool Equals(Variable other) => other != null && (Name, Type, Value).Equals((other.Name, other.Type, other.Value));

        public override bool Equals(ColumnBase other) => Equals(other as Variable);

        public override int GetHashCode() => (Name, Type, Value).GetHashCode();

        public override string ToString() => this.Stringify();
    }
}