using ProvaPub.Enums;
using ProvaPub.Models;
using ProvaPub.Services.Payments;

public class PixPaymentType : IPaymentService
{
    public Task ProcessAsync(Order order, CancellationToken ct = default)
    {
        order.PaymentType = PaymentType.Pix;
        order.PaymentStatus = PaymentStatus.failed;
        return Task.CompletedTask;
    }
}