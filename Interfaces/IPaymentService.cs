using ProvaPub.Enums;
using ProvaPub.Models;

namespace ProvaPub.Services.Payments
{
    public interface IPaymentService
    {             
        Task ProcessAsync(Order order, CancellationToken ct = default);
    }
}
