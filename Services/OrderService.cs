using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Enums;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public interface IOrderService
    {
        Task<Order> PayOrderAsync(int customerId, decimal value, PaymentType method, CancellationToken ct = default);
    }

    public class OrderService : IOrderService
    {
        private readonly TestDbContext _ctx;

        public OrderService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Order> PayOrderAsync(int customerId, decimal value, PaymentType method, CancellationToken ct = default)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));

            var customerExists = await _ctx.Customers.AnyAsync(c => c.Id == customerId, ct);
            if (!customerExists) throw new InvalidOperationException($"Customer {customerId} not found.");

            var order = new Order
            {
                CustomerId = customerId,
                Value = value,
                PaymentType = method,
                PaymentStatus = PaymentStatus.processing,
                OrderDate = DateTime.UtcNow
            };

            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync(ct);

            if (order.OrderDate.Kind != DateTimeKind.Utc)
                order.OrderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);

            var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "E. South America Standard Time"
                : "America/Recife";

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            order.OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone);

            return order;
        }
    }
}
