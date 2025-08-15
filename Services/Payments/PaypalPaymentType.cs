using ProvaPub.Enums;
using ProvaPub.Models;
using ProvaPub.Services.Payments;

public class PaypalPaymentType : IPaymentService
{
    public Task ProcessAsync(Order order, CancellationToken ct = default)
    {
        order.PaymentType = PaymentType.Paypal;
        order.PaymentStatus = PaymentStatus.processing;
        return Task.CompletedTask;
    }
}