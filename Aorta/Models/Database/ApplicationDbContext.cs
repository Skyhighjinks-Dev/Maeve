using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Aorta.Models.Database;
public class ApplicationDbContext : DbContext
{
  /// <summary>Accounts Set</summary>
  public DbSet<Account> Accounts { get; set; }


  /// <summary>
  /// Constructor
  /// </summary>
  /// <param name="nOptions">Options</param>
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> nOptions) : base(nOptions)
  { 
  
  }


  /// <summary>
  /// On Models created, use assembly (IEntityTypeConfiguration<T>)
  /// </summary>
  /// <param name="nModelBuilder">Model builder</param>
  protected override void OnModelCreating(ModelBuilder nModelBuilder)
  {
    nModelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
