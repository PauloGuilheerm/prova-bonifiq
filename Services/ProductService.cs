using Microsoft.EntityFrameworkCore;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService : BasePagingService, IProductService
    {
        private readonly TestDbContext _ctx;

        public ProductService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<PageList<Product>> GetPagedDataAsync(int page, int pageSize = 10, CancellationToken ct = default)
        {
            var query = _ctx.Products
                            .AsNoTracking()
                            .OrderBy(p => p.Id);

            return ToPagedAsync(query, page, pageSize, ct);
        }
    }
}
