using ProvaPub.Enums;
using ProvaPub.Models;
using ProvaPub.Services.Payments;

public class CreditCardPaymentType : IPaymentService
{
    public Task ProcessAsync(Order order, CancellationToken ct = default)
    {
        order.PaymentType = PaymentType.CreditCard;
        order.PaymentStatus = PaymentStatus.paid;
        return Task.CompletedTask;
    }
}