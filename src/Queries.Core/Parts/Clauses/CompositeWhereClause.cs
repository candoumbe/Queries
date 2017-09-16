using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// <para>
    /// Allows to combine several <see cref="IWhereClause"/> instances into a single instance.
    /// </para>
    /// Provides <see cref="Logic"/>
    /// </summary>
    /// <remarks>
    /// Clauses are combined using <see cref="ClauseLogic.And"/> by default.
    /// </remarks>
    public class CompositeWhereClause : IWhereClause, IEquatable<CompositeWhereClause>
    {
        /// <summary>
        /// Logical operator used between each <see cref="Clauses"/>' item.
        /// </summary>
        public ClauseLogic Logic { get; set; }
        /// <summary>
        /// <see cref="IWhereClause"/> instances that are combined
        /// </summary>
        public IEnumerable<IWhereClause> Clauses
        {
            get => _clauses ?? Enumerable.Empty<IWhereClause>();
            set => _clauses = value ?? Enumerable.Empty<IWhereClause>();
        }

        private IEnumerable<IWhereClause> _clauses;


        public bool IsParameterized { get => Clauses.Any(x => x.IsParameterized); }


        /// <summary>
        /// Gets parameters of the <see cref="Clauses"/>
        /// </summary>
        /// <returns>a collection of <see cref="Variable"/>s.</returns>
        public IEnumerable<Variable> GetParameters()
        {
            List<Variable> parameters = new List<Variable>();

            foreach (IWhereClause clause in Clauses)
            {

                if (clause is WhereClause wc && wc.IsParameterized)
                {
                    string parameterName = wc.Column is FieldColumn fc
                            ? !string.IsNullOrWhiteSpace(fc.Alias)
                                ? fc.Alias
                                : fc.Name
                            : $"p{parameters.Count + 1}";

                    switch (wc.Constraint)
                    {
                        case StringColumn sc when sc.Value != null:
                            parameters.Add(new Variable(parameterName, VariableType.String, wc.Constraint));
                            break;
                        case NumericColumn nc when nc.Value != null:
                            parameters.Add(new Variable(parameterName, VariableType.Numeric, wc.Constraint));
                            break;
                        case DateTimeColumn dc when dc.Value != null:
                            parameters.Add(new Variable(parameterName, VariableType.Date, wc.Constraint));
                            break;
                        default:
                            break;
                    }
                }
                else if (clause is CompositeWhereClause cwc)
                {
                    parameters.AddRange(cwc.GetParameters());
                }
            }

            return parameters;
        }

        /// <summary>
        /// Builds a new <see cref="CompositeWhereClause"/> instance.
        /// </summary>
        public CompositeWhereClause() => _clauses = Enumerable.Empty<IWhereClause>();

        public override bool Equals(object obj) => Equals(obj as CompositeWhereClause);
        public bool Equals(CompositeWhereClause other) =>
            other != null
            && Logic == other.Logic
            && Clauses.SequenceEqual(other.Clauses);

        public override int GetHashCode()
        {
            int hashCode = 411513146;
            hashCode = hashCode * -1521134295 + Logic.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<IWhereClause>>.Default.GetHashCode(Clauses);
            return hashCode;
        }
    }
}