using Microsoft.EntityFrameworkCore;
using WebApiMacuco.Models;

namespace WebApiMacuco.Data
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserRecord> UserRecords { get; set; }
  }
}
