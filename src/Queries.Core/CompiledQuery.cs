using Queries.Core.Parts.Clauses;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core;

/// <summary>
/// The result of calling <see cref="Renderers.QueryRendererBase.Compile(IQuery)"/>.
/// </summary>
public class CompiledQuery : IEquatable<CompiledQuery>
{
    /// <summary>
    /// Gets all <see cref="Variable"/>s of the current
    /// </summary>
    public IEnumerable<Variable> Variables { get; }

    /// <summary>
    /// The statement where each variable in <see cref="Variables"/> has a corresponding placeholder.
    /// </summary>
    public string Statement { get; }

    /// <summary>
    /// Builds a new <see cref="CompiledQuery"/> instance
    /// </summary>
    /// <param name="statement"></param>
    /// <param name="variables"></param>
    public CompiledQuery(string statement, IEnumerable<Variable> variables)
    {
        Statement = statement;
        Variables = variables ?? Enumerable.Empty<Variable>();
    }

    ///<inheritdoc/>
    public override bool Equals(object obj) => Equals(obj as CompiledQuery);

    ///<inheritdoc/>
    public bool Equals(CompiledQuery other)
        => other != null && Variables.Intersect(other?.Variables).All(v => Variables.Contains(v)) && Statement == other.Statement;

    ///<inheritdoc/>
#if (NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3)
    public override int GetHashCode() => (Statement, Variables).GetHashCode();
#else
    public override int GetHashCode() => HashCode.Combine(Statement, Variables);
#endif

    /// <summary>
    /// Checks if <paramref name="left"/> is equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> if <paramref name="left"/> is equal to <paramref name="right"/> and <see langword="false"/> otherwise</returns>
    public static bool operator ==(CompiledQuery left, CompiledQuery right)
    {
        return EqualityComparer<CompiledQuery>.Default.Equals(left, right);
    }

    /// <summary>
    /// Checks if <paramref name="left"/> is not equal to <paramref name="right"/>.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns><see langword="true"/> if <paramref name="left"/> is not equal to <paramref name="right"/> and <see langword="false"/> otherwise</returns>
    public static bool operator !=(CompiledQuery left, CompiledQuery right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override string ToString() => this.Jsonify();

    /// <inheritdoc/>
    public void Deconstruct(out IEnumerable<Variable> variables, out string statement)
    {
        variables = Variables;
        statement = Statement;
    }
}