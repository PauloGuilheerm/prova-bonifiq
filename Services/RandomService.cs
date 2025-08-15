using System.Security.Cryptography;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class RandomService
    {
        private readonly TestDbContext _context;

        public RandomService()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Teste;Trusted_Connection=True;")
                .Options;

            _context = new TestDbContext(options);
        }

        public async Task<int> GetRandom()
        {
            const int maxAttempts = 2;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int generatedNumber = RandomNumberGenerator.GetInt32(int.MaxValue);

                bool exists = await _context.Numbers.AnyAsync(x => x.Number == generatedNumber);
                if (exists) continue;

                try
                {
                    _context.Numbers.Add(new RandomNumber { Number = generatedNumber });
                    await _context.SaveChangesAsync();
                    return generatedNumber;
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    continue;
                }
            }

            throw new InvalidOperationException("Unable to generate a unique number at this time. Please try again.");
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }
}
