using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aorta.Models.Database;
public class ApplicationDbContext : DbContext
{
  public DbSet<Account> Accounts { get; set; }

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> nOptions) : base(nOptions)
  { 
  
  }

  protected override void OnModelCreating(ModelBuilder nModelBuilder)
  {
    nModelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
