using Queries.Core;
using Queries.Core.Builders;
using Queries.Core.Parts.Clauses;
using Queries.Core.Parts.Columns;
using Queries.Core.Renderers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static Queries.Core.Builders.Fluent.QueryBuilder;
using static Queries.Core.Parts.Clauses.ClauseOperator;

namespace Queries.Renderers.Sqlite
{
    public class SqliteRenderer : QueryRendererBase
    {
        private const string TablenameTempVariables = "_VARIABLES";

        public SqliteRenderer(QueryRendererSettings settings = null)
            : base(settings ?? new QueryRendererSettings { DateFormatString = "yyyy-MM-dd", PrettyPrint = true })
        { }

        protected override string BeginEscapeWordString => @"""";

        protected override string EndEscapeWordString => @"""";

        protected override string ConcatOperator => "||";

        public override string BatchStatementSeparator => ";";

        protected override string RenderVariable(Variable variable, bool renderAlias) => $"@{variable.Name}";

        public override string Render(IQuery query)
        {
            string result = string.Empty;
            CollectVariableVisitor visitor = new CollectVariableVisitor();
            switch (query)
            {
                case SelectQueryBase selectQueryBase:
                    if (selectQueryBase is SelectQuery sq)
                    {
                        visitor.Visit(sq);
                        result = Render(sq);
                    }
                    result = Render(selectQueryBase);
                    break;
                case CreateViewQuery createViewQuery:
                    result = Render(createViewQuery);
                    break;
                case DeleteQuery deleteQuery:
                    visitor.Visit(deleteQuery);
                    result = Render(deleteQuery);
                    break;
                case UpdateQuery updateQuery:
                    result = Render(updateQuery);
                    break;
                case TruncateQuery truncateQuery:
                    result = Render(truncateQuery);
                    break;
                case InsertIntoQuery insertIntoQuery:
                    result = Render(insertIntoQuery);
                    break;
                case BatchQuery batchQuery:
                    result = Render(batchQuery);
                    break;
                default:
                    result = base.Render(query);
                    break;
            }
            StringBuilder sbParameters = new StringBuilder(visitor.Variables.Count() * 100);
            if (visitor.Variables.Any())
            {
#if DEBUG
                Debug.Assert(visitor.Variables.All(x => x.Value != null), $"{nameof(visitor)}.{nameof(visitor.Variables)} must not contains variables with null value");
#endif

                IList<InsertIntoQuery> insertParameters = new List<InsertIntoQuery>(visitor.Variables.Count());
                IList<UpdateQuery> updateParameters = new List<UpdateQuery>(visitor.Variables.Count());

                foreach (Variable variable in visitor.Variables)
                {
                    insertParameters.Add(
                        InsertInto(TablenameTempVariables).Values("ParameterName".Field().InsertValue(variable.Name.Literal()))
                        .Build()
                    );
                    switch (variable.Type)
                    {
                        case VariableType.Numeric:
                            updateParameters.Add(
                                   Update(TablenameTempVariables).Set("RealValue".Field().UpdateValueTo(Convert.ToInt64(variable.Value)))
                                   .Where("ParameterName".Field().EqualTo(variable.Name))
                            );
                            break;
                        case VariableType.Date:
                        case VariableType.String:
                            updateParameters.Add(
                                Update(TablenameTempVariables).Set("TextValue".Field().UpdateValueTo(variable.Value.ToString()))
                                    .Where("ParameterName".Field().EqualTo(variable.Name))
                            );
                            break;
                        case VariableType.Boolean:
                            updateParameters.Add(
                                Update(TablenameTempVariables).Set("IntegerValue".Field().UpdateValueTo(Convert.ToInt32(variable.Value)))
                                    .Where("ParameterName".Field().EqualTo(variable.Name))
                            );
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Unsupported {variable.Type} as variable type");
                    }

                    
                }
                BatchQuery batch = new BatchQuery()
                    .AddStatement("BEGIN;".AsNative())
                    .AddStatement("PRAGMA temp_store=2;".AsNative())
                    .AddStatement($"CREATE TEMP TABLE {RenderTablename(TablenameTempVariables.Table(), renderAlias:false)}(ParameterName TEXT, RealValue REAL, IntegerValue INTEGER, BlobValue BLOB);"
                        .AsNative()
                    )
                    .AddStatements(insertParameters)
                    .AddStatements(updateParameters)
                    .AddStatement(result.AsNative())
                    .AddStatement($"DROP TABLE {RenderTablename(TablenameTempVariables.Table(), renderAlias: false)};".AsNative())
                    .AddStatement("END".AsNative());
            }
            return sbParameters.Append(result).ToString();
        }
    }
}
