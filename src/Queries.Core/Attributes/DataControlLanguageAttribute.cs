using System;

namespace Queries.Core.Attributes;

/// <summary>
/// Marks a statement that is used for managing rights within schema objects. 
/// (see http://www.orafaq.com/faq/what_are_the_difference_between_ddl_dml_and_dcl_commands).
/// </summary>
/// <remarks>
/// This kind of query cannot be rollbacked
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public sealed class DataControlLanguageAttribute : Attribute
{        
}
