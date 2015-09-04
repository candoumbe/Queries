﻿using System;
using System.Linq;
using Queries.Builders;

namespace Queries.Renderers
{
    public class SqlServerRenderer : SqlRendererBase
    {
        public override string Render(SelectQueryBase query)
        {
            string result = Render(query, DatabaseType.SqlServer);
            return result;
        }

        protected override string GetConcatString()
        {
            return "+";
        }

        public override string EscapeName(string rawColumnName)
        {

            string escapedColumnName = String.Join(".",
                rawColumnName.Split(new[] {'.'}, StringSplitOptions.None)
                .Select(item => String.Format("[{0}]", item)));

            return escapedColumnName;
        }

    }


    public static class SqlQueryExtensions
    {
        public static string ToSql(this SelectQuery selectQuery, DatabaseType type)
        {
            if (selectQuery == null)
            {
                throw new ArgumentNullException("selectQuery");
            }
            
            string query = String.Empty;
            
            switch (type)
            {
                case DatabaseType.SqlServer:
                case DatabaseType.SqlServerCompact:
                    query = new SqlServerRenderer().Render(selectQuery);    
                    break;
                case DatabaseType.Mysql:
                case DatabaseType.MariaDb:
                case DatabaseType.Postgres:
                    query = new PostgresqlRenderer().Render(selectQuery);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }


            return query;
        }
    }
}