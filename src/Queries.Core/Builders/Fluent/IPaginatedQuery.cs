namespace Queries.Core.Builders.Fluent;

/// <summary>
/// Fluently adds paging support to <typeparamref name="TQuery"/>.
/// </summary>
/// <typeparam name="TQuery">Type of the query onto which the paging options will be applied</typeparam>
public interface IPaginatedQuery<out TQuery>: IBuild<TQuery>
{
    /// <summary>
    /// Adds pagination parameters
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns><typeparamref name="TQuery"/> onto which paging options were applied.</returns>
    TQuery Paginate(int pageIndex, int pageSize);
}