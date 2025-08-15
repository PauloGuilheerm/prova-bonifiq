using ProvaPub.Enums;

namespace ProvaPub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public PaymentType PaymentType { get; set; } = default!;
        public PaymentStatus PaymentStatus { get; set; } = default!;

        public Customer Customer { get; set; } = default!;
    }
}
