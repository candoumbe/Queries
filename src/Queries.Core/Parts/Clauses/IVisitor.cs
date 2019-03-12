namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of the visited element.</typeparam>
    public interface IVisitor<T>
    {
        /// <summary>
        /// Visits the specified instance
        /// </summary>
        /// <param name="instance"></param>
        void Visit(T instance);
    }
}
