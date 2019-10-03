namespace Queries.Core.Parts.Clauses
{
    /// <summary>
    /// Inteface to implement when visiting a <see cref="T"/> instance.
    /// </summary>
    /// <typeparam name="T">Type of the visited element.</typeparam>
    public interface IVisitor<T>
    {
        /// <summary>
        /// Visits the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance"></param>
        void Visit(T instance);
    }
}
