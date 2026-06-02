using EMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<ApplicationLog> ApplicationLogs { get; set; } = null!;
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; } = null!;
    }
}
