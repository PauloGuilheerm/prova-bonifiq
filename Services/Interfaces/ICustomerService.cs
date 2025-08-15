using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<PageList<Customer>> GetPagedDataAsync(int page, int pageSize = 10, CancellationToken ct = default);
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
