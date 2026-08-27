using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
using System.Threading.Tasks;

namespace ReceptionSystem.Services
{
    public class JobApplicationNumberGenerator
    {
        private readonly ApplicationDbContext _context;

        public JobApplicationNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNextNumberAsync(string department = "HR")
        {
            var nextValueParam = new SqlParameter
            {
                ParameterName = "@nextVal",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            await _context.Database.ExecuteSqlRawAsync(
                "SET @nextVal = NEXT VALUE FOR dbo.JobApplicationSeq",
                nextValueParam);

            int nextNumber = (int)nextValueParam.Value;

            return $"DAMA/{department}/{nextNumber}";
        }
    }
}