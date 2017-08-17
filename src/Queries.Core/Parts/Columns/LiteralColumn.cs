using Queries.Core.Builders;
using System;

namespace Queries.Core.Parts.Columns
{
    /// <summary>
    /// A column that render is content as is with no interpretation.
    /// </summary>
    public class LiteralColumn : ColumnBase, IAliasable<LiteralColumn>, IInsertable
    {
        public object Value { get; set; }

        /// <summary>
        /// Builds a <see cref="LiteralColumn"/> instance.
        /// </summary>
        /// <param name="value"></param>
        public LiteralColumn(object value = null)
        {
            switch (value)
            {
                case int i:
                    Value = i;
                    break;
                case float f:
                    Value = f;
                    break;
                case double d:
                    Value = d;
                    break;
                case long l:
                    Value = l;
                    break;
                case bool b:
                    Value = b;
                    break;
                case string s:
                    Value = s;
                    break;
                case DateTime dateTime:
                    Value = dateTime;
                    break;
                case DateTimeOffset dateTimeOffset:
                    Value = dateTimeOffset;
                    break;
                case null:
                    Value = value;
                    break;
                default:
                    throw new ArgumentException(nameof(value), "only bool/int/float/double/long/");
            }
        }

        private string _alias;

        public string Alias => _alias;

        public LiteralColumn As(string alias)
        {
            _alias = alias;

            return this;
        }



    }
}
