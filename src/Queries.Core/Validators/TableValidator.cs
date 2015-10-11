using System;
using Queries.Core.Parts;

namespace Queries.Core.Validators
{
    public class TableValidator : IValidate<ITable>
    {



        public bool IsValid(ITable table)
        {
            Func<ITable, bool> validateFunc = t =>
            {
                bool valid = false;

                if (table is Table)
                {
                    valid = ! String.IsNullOrWhiteSpace(((Table) table).Name);
                }
                else if (table is SelectTable)
                {
                    SelectTable selectTable = (SelectTable) table;
                    valid = new SelectQueryValidator().IsValid(selectTable.Select);
                }


                return valid;
            };

            return validateFunc(table);
        }
    }
}