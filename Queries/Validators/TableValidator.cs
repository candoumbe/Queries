using System;
using Queries.Parts;

namespace Queries.Validators
{
    public class TableValidator : IValidate<ITable>
    {

        public bool Validate(ITable table)
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
                    valid = new SelectQueryValidator().Validate(selectTable.Select);
                }


                return valid;
            };

            return validateFunc(table);
        }
    }
}