using Microsoft.EntityFrameworkCore;
using TienDat_ApiResult.Models;
namespace TienDat_ApiResult.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public ApiContext(DbContextOptions<ApiContext> optison) : base(optison)
        {

        }
    }
}
