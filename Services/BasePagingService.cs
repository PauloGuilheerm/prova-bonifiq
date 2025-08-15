using ProvaPub.Models;

namespace ProvaPub.Services
{
    public abstract class BasePagingService
    {
        protected Task<PageList<T>> ToPagedAsync<T>(IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
            => PageList<T>.CreateAsync(query, page, pageSize, ct);
    }
}
