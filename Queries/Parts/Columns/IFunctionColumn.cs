using System.Collections.Generic;
using System.Linq;

namespace Queries.Parts.Columns
{
    public interface IFunctionColumn : IColumn
    {}

    public class Concat : IFunctionColumn
    {
        public IList<IColumn> Columns { get; private set; }

        public Concat(params IColumn[] columns)
        {
            Columns = columns == null ? new List<IColumn>() : columns.ToList();
        }

    }
}