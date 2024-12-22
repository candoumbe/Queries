using Queries.Core.Parts.Clauses;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core;

/// <summary>
/// Represents a query that was compiled alognside its variables
/// </summary>
public class CompiledQuery : IEquatable<CompiledQuery>
{
    /// <summary>
    /// Gets all <see cref="Variable"/>s of the current <see cref="CompiledQuery"/>.
    /// </summary>
    public IReadOnlyList<Variable> Variables { get; }

    /// <summary>
    /// The statement where each variable in <see cref="Variables"/> has a corresponding placeholder.
    /// </summary>
    public string Statement { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledQuery"/> class.
    /// </summary>
    /// <param name="statement">The statement of the compiled query.</param>
    /// <param name="variables">The variables of the compiled query.</param>
    public CompiledQuery(string statement, IReadOnlyList<Variable> variables)
    {
        Statement = statement;
        Variables = variables ?? [];
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as CompiledQuery);

    /// <inheritdoc />
    public bool Equals(CompiledQuery other)
        => other is not null && Variables.Intersect(other.Variables).All(v => Variables.Contains(v)) && Statement == other.Statement;

    /// <inheritdoc />
#if (NETSTANDARD)
    public override int GetHashCode() => (Statement, Variables).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Statement, Variables);
#endif

    /// <summary>
    /// Determines whether two specified <see cref="CompiledQuery"/> objects are equal.
    /// </summary>
    /// <param name="left">The first <see cref="CompiledQuery"/> to compare.</param>
    /// <param name="right">The second <see cref="CompiledQuery"/> to compare.</param>
    /// <returns>true if <paramref name="left"/> is equal to <paramref name="right"/>; otherwise, false.</returns>
    public static bool operator ==(CompiledQuery left, CompiledQuery right) => EqualityComparer<CompiledQuery>.Default.Equals(left, right);

    /// <summary>
    /// Determines whether two specified <see cref="CompiledQuery"/> objects are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="CompiledQuery"/> to compare.</param>
    /// <param name="right">The second <see cref="CompiledQuery"/> to compare.</param>
    /// <returns>true if <paramref name="left"/> is not equal to <paramref name="right"/>; otherwise, false.</returns>
    public static bool operator !=(CompiledQuery left, CompiledQuery right) => !(left == right);

    /// <summary>
    /// Returns a string that represents the current <see cref="CompiledQuery"/>.
    /// </summary>
    /// <returns>A string that represents the current <see cref="CompiledQuery"/>.</returns>
    public override string ToString() => this.Jsonify();

    /// <summary>
    /// Deconstructs the current <see cref="CompiledQuery"/> into its variables and statement.
    /// </summary>
    /// <param name="variables">When this method returns, contains the variables of the current <see cref="CompiledQuery"/>.</param>
    /// <param name="statement">When this method returns, contains the statement of the current <see cref="CompiledQuery"/>.</param>
    public void Deconstruct(out IReadOnlyList<Variable> variables, out string statement)
    {
        variables = Variables;
        statement = Statement;
    }
}