using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using System;
using System.Collections.Generic;
using System.Linq;
using static Queries.Core.Parts.Clauses.VariableType;

namespace Queries.Core.Builders
{
    /// <summary>
    /// Collects <see cref="Variable"/>s from queries and rewrites each literal so that it's replaces
    /// the collected variables.
    /// </summary>
    public class CollectVariableVisitor : IVisitor<SelectQuery>, IVisitor<IWhereClause>, IVisitor<InsertIntoQuery>, IVisitor<DeleteQuery>
    {
        public IEnumerable<Variable> Variables { get; set; }

        /// <summary>
        /// Builds a new <see cref="CollectVariableVisitor"/> instance.
        /// </summary>
        public CollectVariableVisitor() => Variables = Enumerable.Empty<Variable>();

        public void Visit(SelectQuery instance)
        {
            foreach (ITable item in instance.Tables)
            {
                switch (item)
                {
                    case SelectTable st:
                        Visit(((SelectTable)item).Select);
                        break;
                    case SelectQuery sq:
                        Visit(sq);
                        break;
                }
            }
            Visit(instance.WhereCriteria);
            foreach (SelectQuery unionQuery in instance.Unions)
            {
                Visit(unionQuery);
            }
        }

        public virtual void Visit(IWhereClause instance)
        {
            switch (instance)
            {
                case WhereClause wc when wc.Constraint != null:
                    switch (wc.Constraint)
                    {
                        case StringColumn sc when sc.Value != null:
                            {
                                Variable variable = Variables.SingleOrDefault(x => x.Type == VariableType.String && Equals(sc.Value, x.Value));
                                if (variable is null)
                                {
                                    variable = new Variable($"p{Variables.Count()}", VariableType.String, sc.Value);
                                    Variables = Variables.Concat(new[] { variable });
                                }
                                ((WhereClause)instance).Constraint = variable;
                            }
                            break;

                        case BooleanColumn bc when bc.Value != null:
                            {
                                Variable variable = Variables.SingleOrDefault(x => x.Type == VariableType.Boolean && bc.Value == x.Value);
                                if (variable is null)
                                {
                                    variable = new Variable($"p{Variables.Count()}", VariableType.Boolean, bc.Value);
                                    Variables = Variables.Concat(new[] { variable });
                                }
                                ((WhereClause)instance).Constraint = variable;
                            }
                            break;

                        case DateTimeColumn dc when dc.Value != null:
                            {
                                Variable variable = Variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);
                                if (variable is null)
                                {
                                    variable = new Variable($"p{Variables.Count()}", Date, dc.Value);
                                    Variables = Variables.Concat(new[] { variable });
                                }
                                ((WhereClause)instance).Constraint = variable;
                            }
                            break;
                        case StringValues strings:
                            IEnumerable<Variable> variables = Enumerable.Empty<Variable>();
                            foreach (string value in strings)
                            {
                                Variable variable = Variables.SingleOrDefault(x => x.Type == VariableType.String && Equals(value,x.Value));
                                if (variable is null)
                                {
                                    variable = new Variable($"p{Variables.Count()}", VariableType.String, value);
                                    variables = variables.Concat(new[] { variable });
                                    Variables = Variables.Concat(new[] { variable });
                                }
                            }
                            if (variables.Any())
                            {
                                ((WhereClause)instance).Constraint = new VariableValues(variables.First(), variables.Skip(1).ToArray());
                            }
                            break;
                        case NumericColumn nc when nc.Value != null:
                            {
                                Variable variable = Variables.SingleOrDefault(x => x.Type == Numeric && nc.Value == x.Value);
                                if (variable is null)
                                {
                                    variable = new Variable($"p{Variables.Count()}", Numeric, nc.Value);
                                    Variables = Variables.Concat(new[] { variable });
                                }
                                ((WhereClause)instance).Constraint = variable;
                            }
                            break;
                    }

                    break;
                case CompositeWhereClause cwc:
                    foreach (IWhereClause item in cwc.Clauses)
                    {
                        Visit(item);
                    }
                    break;
            }
        }

        public void Visit(InsertIntoQuery instance)
        {
            if (instance.InsertedValue is InsertedValues)
            {
                foreach (InsertedValue item in (InsertedValues)instance.InsertedValue)
                {
                    switch (item.Value)
                    {
                        case StringColumn sc when sc.Value != null:
                            {
                                Variable variable = new Variable($"p{Variables.Count()}", VariableType.String, sc.Value);
                                Variables = Variables.Concat(new[] { variable });
                                item.Value = variable;
                            }
                            break;

                        case BooleanColumn bc when bc.Value != null:
                            {
                                Variable variable = new Variable($"p{Variables.Count()}", VariableType.Boolean, bc.Value);
                                Variables = Variables.Concat(new[] { variable });
                                item.Value = variable;
                            }
                            break;

                        case DateTimeColumn dc when dc.Value != null:
                            {
                                Variable variable = new Variable($"p{Variables.Count()}", Date, dc.Value);
                                Variables = Variables.Concat(new[] { variable });
                                item.Value = variable;
                            }
                            break;
                    }
                }
            }
        }

        public void Visit(DeleteQuery instance)
        {
            if (instance.Criteria != null)
            {
                Visit(instance.Criteria);
            }
        }
    }
}
