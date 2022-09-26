using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Queries.Renderers.Sqlite;

/// <summary>
/// A visitor that can rewrite <see cref="IQuery"/>'s variables and replaces each of them with a <see cref="SelectQuery"/>
/// </summary>
public class ReplaceParameterBySelectQueryVisitor : IVisitor<IQuery>, IVisitor<IWhereClause>
{
    private readonly Func<Variable, SelectQuery> _variableRewriter;
    private readonly CollectVariableVisitor _variableVisitor;

    /// <summary>
    /// Builds a new <see cref="ReplaceParameterBySelectQueryVisitor"/> instance
    /// </summary>
    /// <param name="variableRewriter">Defines how a variable should be rewritten</param>
    public ReplaceParameterBySelectQueryVisitor(Func<Variable, SelectQuery> variableRewriter)
    {
        _variableRewriter = variableRewriter ?? throw new ArgumentNullException(nameof(variableRewriter));
        _variableVisitor = new CollectVariableVisitor();
    }

    ///<inheritdoc/>
    public IEnumerable<Variable> Variables => _variableVisitor.Variables;

    ///<inheritdoc/>
    public void Visit(IQuery instance)
    {
        if (instance is SelectQuery selectQuery)
        {
            _variableVisitor.Visit(selectQuery);
            if (selectQuery.WhereCriteria != null)
            {
                Visit(selectQuery.WhereCriteria);
            }

        }
    }

    ///<inheritdoc/>
    public virtual void Visit(IWhereClause instance)
    {
        switch (instance)
        {
            case WhereClause wc when wc.Constraint is Variable v:
                bool canBeReplace = _variableVisitor.Variables.Any(variable => variable == v);
                if (canBeReplace)
                {
                    ((WhereClause)instance).Constraint = _variableRewriter.Invoke(v);
                }
                break;
            case CompositeWhereClause cwc:
                foreach (IWhereClause item in cwc.Clauses)
                {
                    Visit(item);
                }
                break;
            default:
                break;
        }
    }
}
