using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Services;
using Xunit;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private static TestDbContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase($"tests-{Guid.NewGuid()}")
                .Options;

            return new TestDbContext(options);
        }

        private static bool IsBusinessHoursNowUtc()
        {
            var now = DateTime.UtcNow;
            var inHours = now.Hour >= 8 && now.Hour <= 18;
            var isWorkday = now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday;
            return inHours && isWorkday;
        }

        [Fact(DisplayName = "Should throw when customerId <= 0")]
        public async Task Should_throw_when_customerId_is_non_positive()
        {
            using var ctx = CreateDb();
            var service = new CustomerService(ctx);

            var act = () => service.CanPurchase(0, 10m);
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("customerId");
        }

        [Fact(DisplayName = "Should throw when purchaseValue <= 0")]
        public async Task Should_throw_when_purchaseValue_is_non_positive()
        {
            using var ctx = CreateDb();
            var service = new CustomerService(ctx);

            var act = () => service.CanPurchase(1, 0m);
            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("purchaseValue");
        }

        [Fact(DisplayName = "Should throw when customer does not exist")]
        public async Task Should_throw_when_customer_not_found()
        {
            using var ctx = CreateDb();
            var service = new CustomerService(ctx);

            var act = () => service.CanPurchase(999, 50m);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*does not exists*");
        }

        [Fact(DisplayName = "Should return false when customer has an order within the last month")]
        public async Task Should_return_false_if_bought_in_last_month()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });

            ctx.Orders.Add(new Order
            {
                Id = 10,
                CustomerId = 1,
                Value = 30m,
                OrderDate = DateTime.UtcNow.AddDays(-1)
            });
            await ctx.SaveChangesAsync();

            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 50m);

            result.Should().BeFalse("a customer can purchase only once per month");
        }

        [Fact(DisplayName = "Should treat order exactly at 1-month boundary as within the last month")]
        public async Task Should_block_order_at_exact_boundary()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });

            ctx.Orders.Add(new Order
            {
                Id = 11,
                CustomerId = 1,
                Value = 30m,
                OrderDate = DateTime.UtcNow.AddMonths(-1)
            });
            await ctx.SaveChangesAsync();

            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 50m);

            result.Should().BeFalse();
        }

        [Fact(DisplayName = "Should return false on first purchase when value > 100")]
        public async Task Should_block_first_purchase_over_100()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 150m);

            result.Should().BeFalse("first purchase is limited to 100.00");
        }

        [Fact(DisplayName = "Should allow first purchase up to 100 only during business hours (time-dependent)")]
        public async Task Should_allow_first_purchase_up_to_100_time_dependent()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            var expected = IsBusinessHoursNowUtc();
            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 80m);

            result.Should().Be(expected,
                "first purchase up to 100 is only allowed during business hours and workdays");
        }

        [Fact(DisplayName = "Should allow purchases over 100 for customers with prior history older than 1 month (time-dependent)")]
        public async Task Should_allow_over_100_when_has_history_outside_last_month_time_dependent()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });

            ctx.Orders.Add(new Order
            {
                Id = 20,
                CustomerId = 1,
                Value = 40m,
                OrderDate = DateTime.UtcNow.AddMonths(-2)
            });
            await ctx.SaveChangesAsync();

            var expected = IsBusinessHoursNowUtc();
            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 200m);

            result.Should().Be(expected,
                "the 100.00 cap applies only to first purchase; afterwards it's gated by business hours");
        }

        [Fact(DisplayName = "Should allow purchase when no order in last month and value <= 100 (time-dependent)")]
        public async Task Should_allow_when_no_recent_orders_and_value_within_limit_time_dependent()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });

            ctx.Orders.Add(new Order
            {
                Id = 30,
                CustomerId = 1,
                Value = 10m,
                OrderDate = DateTime.UtcNow.AddMonths(-3)
            });
            await ctx.SaveChangesAsync();

            var expected = IsBusinessHoursNowUtc();
            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 90m);

            result.Should().Be(expected);
        }

        [Fact(DisplayName = "Should be blocked outside business hours and on weekends (time-dependent)")]
        public async Task Should_block_outside_business_hours_time_dependent()
        {
            using var ctx = CreateDb();
            ctx.Customers.Add(new Customer { Id = 1, Name = "Test" });
            await ctx.SaveChangesAsync();

            var expected = IsBusinessHoursNowUtc();
            var service = new CustomerService(ctx);
            var result = await service.CanPurchase(1, 50m);

            result.Should().Be(expected);
        }
    }
}
