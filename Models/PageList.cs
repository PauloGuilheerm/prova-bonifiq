using Microsoft.EntityFrameworkCore;

namespace ProvaPub.Models
{
    public class PageList<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public bool HasNext => Page * PageSize < TotalCount;
        public bool HasPrevious => Page > 1;

        public PageList(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public static async Task<PageList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PageList<T>(items, page, pageSize, total);
        }
    }
}
