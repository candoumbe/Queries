﻿using Queries.Core.Parts.Columns;

using System;

namespace Queries.Core.Parts.Clauses;

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
    /// <exception cref="ArgumentNullException">if <paramref name="name"/> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">if <paramref name="name"/> is <c>empty</c></exception>
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

    ///<inheritdoc/>
    public override IColumn Clone() => new Variable(Name, Type, Value);

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as Variable);

    ///<inheritdoc/>
    public bool Equals(Variable other) => Name.Equals(other?.Name) 
                                          && Type == other?.Type
                                          && Equals(Value, other?.Value);

    ///<inheritdoc/>
    public override bool Equals(ColumnBase other) => Equals(other as Variable);

    ///<inheritdoc/>
#if !NETSTANDARD2_1
    public override int GetHashCode() => (Name, Type, Value).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Name, Type, Value);
#endif
    ///<inheritdoc/>
    public override string ToString() => this.Jsonify();
}