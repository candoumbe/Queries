using System;
using Queries.Core.Parts;

namespace Queries.Core.Validators
{
    public class TableValidator : IValidate<ITable>
    {



        public bool IsValid(ITable table)
        {
            bool valid = false;

            if (table is Table t)
            {
                valid = !string.IsNullOrWhiteSpace(t.Name);
            }
            else if (table is SelectTable selectTable)
            {
                valid = new SelectQueryValidator().IsValid(selectTable.Select);
            }
            return valid;

        }
    }
}