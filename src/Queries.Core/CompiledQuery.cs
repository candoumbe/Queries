﻿using Queries.Core.Parts.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core
{
    /// <summary>
    /// The result of calling <see cref="Renderers.QueryRendererBase.Compile(IQuery)"/>.
    /// </summary>
    public class CompiledQuery : IEquatable<CompiledQuery>
    {
        public IEnumerable<Variable> Variables { get; }

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

        public override bool Equals(object obj) => Equals(obj as CompiledQuery);

        public bool Equals(CompiledQuery other)
            => other != null && Variables.Intersect(other?.Variables).All(v => Variables.Contains(v)) && Statement == other.Statement;

#if (NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_3)
        public override int GetHashCode() => (Statement, Variables).GetHashCode();
#else
        public override int GetHashCode() => HashCode.Combine(Statement, Variables);
#endif

        public static bool operator ==(CompiledQuery left, CompiledQuery right)
        {
            return EqualityComparer<CompiledQuery>.Default.Equals(left, right);
        }

        public static bool operator !=(CompiledQuery left, CompiledQuery right)
        {
            return !(left == right);
        }

        public override string ToString() => this.Jsonify();
    }
}