namespace Queries.Core
{
    /// <summary>
    /// This interface marks a statement that is used for managing data within schema objects. 
    /// (see http://www.orafaq.com/faq/what_are_the_difference_between_ddl_dml_and_dcl_commands)
    /// </summary>
    public interface IDataManipulationQuery : IQuery
    { }
}