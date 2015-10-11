using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Queries.Core.Parts.Columns
{
    public interface IFormattableColumn<out T> where T : IColumn
    {
       T Format(string format);
    }
}
