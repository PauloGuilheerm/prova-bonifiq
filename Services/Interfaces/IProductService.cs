using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface IProductService
    {
        Task<PageList<Product>> GetPagedDataAsync(int page, int pageSize = 10, CancellationToken ct = default);
    }
}
