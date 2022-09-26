using Queries.Core.Parts;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;

using System;
using System.Collections.Generic;
using System.Linq;

using static Queries.Core.Parts.Clauses.VariableType;

namespace Queries.Core.Builders;

/// <summary>
/// Collects <see cref="Literal"/>s from queries and replaces each of them by a <see cref="Variable"/> counterpart. 
/// </summary>
public class CollectVariableVisitor : IVisitor<SelectQuery>, IVisitor<IWhereClause>, IVisitor<InsertIntoQuery>, IVisitor<DeleteQuery>
{
    /// <summary>
    /// Collection of variables that 
    /// </summary>
    public IEnumerable<Variable> Variables
#if NETSTANDARD
        => _variables.ToArray();
#else
        => _variables.AsReadOnly();
#endif
    private readonly List<Variable> _variables;

    /// <summary>
    /// Builds a new <see cref="CollectVariableVisitor"/> instance.
    /// </summary>
    public CollectVariableVisitor() => _variables = new();

    ///<inheritdoc/>
    public void Visit(SelectQuery instance)
    {
        foreach (CasesColumn item in instance.Columns.OfType<CasesColumn>())
        {
            foreach (WhenExpression when in item.Cases)
            {
                Visit(when.Criterion);
                switch (when.ThenValue)
                {
                    case BooleanColumn bc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.Boolean && bc.Value == x.Value);
                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", VariableType.Boolean, bc.Value);
                                _variables.Add(variable);
                            }

                            when.ThenValue = variable;
                        }
                        break;
                    case DateTimeColumn dc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);

                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                                _variables.Add(variable);
                            }

                            when.ThenValue = variable;
                        }
                        break;
#if NET6_0_OR_GREATER
                    case DateColumn dc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);

                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                                _variables.Add(variable);
                            }

                            when.ThenValue = variable;
                        }
                        break;
#endif
                    case NumericColumn nc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Numeric && nc.Value == x.Value);

                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", Numeric, nc.Value);
                                _variables.Add(variable);
                            }

                            when.ThenValue = variable;
                        }
                        break;
                    case StringColumn sc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.String && sc.Value == x.Value);

                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", VariableType.String, sc.Value);
                                _variables.Add(variable);
                            }

                            when.ThenValue = variable;
                        }
                        break;
                }
            }

            switch (item.Default)
            {
                case BooleanColumn bc:
                    {
                        Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.Boolean && bc.Value == x.Value);
                        if (variable is null)
                        {
                            variable = new Variable($"p{_variables.Count}", VariableType.Boolean, bc.Value);
                            _variables.Add(variable);
                        }
                        item.Default = variable;
                    }
                    break;
                case DateTimeColumn dc:
                    {
                        Variable variable = _variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);
                        if (variable is null)
                        {
                            variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                            _variables.Add(variable);
                        }
                        item.Default = variable;
                    }
                    break;
                case NumericColumn nc:
                    {
                        Variable variable = _variables.SingleOrDefault(x => x.Type == Numeric && nc.Value == x.Value);
                        if (variable is null)
                        {
                            variable = new Variable($"p{_variables.Count}", Numeric, nc.Value);
                            _variables.Add(variable);
                        }
                        item.Default = variable;
                    }
                    break;
                case StringColumn sc:
                    {
                        Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.String && sc.Value == x.Value);
                        if (variable is null)
                        {
                            variable = new Variable($"p{_variables.Count}", VariableType.String, sc.Value);
                            _variables.Add(variable);
                        }
                        item.Default = variable;
                    }
                    break;
            }
        }
        foreach (ITable item in instance.Tables)
        {
            switch (item)
            {
                case SelectTable st:
                    Visit(st.Select);
                    break;
                case SelectQuery sq:
                    Visit(sq);
                    break;
            }
        }
        if (instance.WhereCriteria is not null)
        {
            Visit(instance.WhereCriteria);
        }
        foreach (SelectQuery unionQuery in instance.Unions)
        {
            Visit(unionQuery);
        }
    }

    ///<inheritdoc/>
    public virtual void Visit(IWhereClause instance)
    {
        switch (instance)
        {
            case WhereClause wc:
                switch (wc.Constraint)
                {
                    case NumericColumn nc:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Numeric && nc.Value == x.Value);
                            if (variable == null)
                            {
                                variable = new Variable($"p{_variables.Count}", Numeric, nc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;

                    case StringColumn sc when sc.Value is not null:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.String && Equals(sc.Value, x.Value));
                            if (variable is null)
                            {
                                variable = new Variable($"p{_variables.Count}", VariableType.String, sc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;

                    case BooleanColumn bc when bc.Value is not null:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.Boolean && bc.Value == x.Value);
                            if (variable is null)
                            {
                                variable = new Variable($"p{_variables.Count}", VariableType.Boolean, bc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;

                    case DateTimeColumn dc when dc.Value is not null:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);
                            if (variable is null)
                            {
                                variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;
#if NET6_0_OR_GREATER
                    case DateColumn dc when dc.Value is not null:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Date && dc.Value == x.Value);
                            if (variable is null)
                            {
                                variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;

                    case TimeColumn tc when tc.Value is not null:
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == Time && tc.Value == x.Value);
                            if (variable is null)
                            {
                                variable = new Variable($"p{_variables.Count}", Time, tc.Value);
                                _variables.Add(variable);
                            }
                            wc.Constraint = variable;
                        }
                        break;
#endif
                    case StringValues strings:
                        IList<Variable> stringValueVariables = new List<Variable>();
                        foreach (string value in strings)
                        {
                            Variable variable = _variables.SingleOrDefault(x => x.Type == VariableType.String && Equals(value, x.Value));
                            if (variable is null)
                            {
                                variable = new Variable($"p{Variables.Count()}", VariableType.String, value);
                                stringValueVariables.Add(variable);
                                _variables.Add(variable);
                            }
                        }
                        if (stringValueVariables.Any())
                        {
                            wc.Constraint = new VariableValues(stringValueVariables.First(), stringValueVariables.Skip(1).ToArray());
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

            default:
                throw new ArgumentOutOfRangeException(nameof(instance), instance, $"Unexpected {instance.GetType()} clause type");
        }
    }

    ///<inheritdoc/>
    public void Visit(InsertIntoQuery instance)
    {
        if (instance.InsertedValue is InsertedValues insertValues)
        {
            foreach (InsertedValue item in insertValues)
            {
                switch (item.Value)
                {
                    case StringColumn sc when sc.Value is not null:
                        {
                            Variable variable = new Variable($"p{_variables.Count}", VariableType.String, sc.Value);
                            _variables.Add(variable);
                            item.Value = variable;
                        }
                        break;

                    case BooleanColumn bc when bc.Value is not null:
                        {
                            Variable variable = new Variable($"p{_variables.Count}", VariableType.Boolean, bc.Value);
                            _variables.Add(variable);
                            item.Value = variable;
                        }
                        break;

                    case DateTimeColumn dc when dc.Value is not null:
                        {
                            Variable variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                            _variables.Add(variable);
                            item.Value = variable;
                        }
                        break;
#if NET6_0_OR_GREATER
                    case DateColumn dc when dc.Value is not null:
                        {
                            Variable variable = new Variable($"p{_variables.Count}", Date, dc.Value);
                            _variables.Add(variable);
                            item.Value = variable;
                        }
                        break;
#endif
                }
            }
        }
    }

    ///<inheritdoc/>
    public void Visit(DeleteQuery instance)
    {
        if (instance.Criteria is not null)
        {
            Visit(instance.Criteria);
        }
    }
}
