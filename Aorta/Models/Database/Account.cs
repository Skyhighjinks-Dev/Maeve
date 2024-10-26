using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aorta.Models.Database;
public class Account 
{
  /// <summary>Internal Account ID</summary>
  public int ID { get; set; }

  /// <summary>User defined username</summary>
  public string Username { get; set; }

  /// <summary>User defined pasword (should probably follow good security protocols)</summary>
  public string Password { get; set; }

  /// <summary>User email</summary>
  public string Email { get; set; }

  /// <summary>Created At</summary>
  public DateTime CreatedAt { get; set; }

  /// <summary>Record last updated at</summary>
  public DateTime UpdatedAt { get; set; }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
  /// <summary>
  /// Configures Accounts table
  /// </summary>
  /// <param name="nBuilder">Type builder</param>
  public void Configure(EntityTypeBuilder<Account> nBuilder)
  {
    nBuilder.HasKey(x => x.ID);
    nBuilder.Property(x => x.ID) 
            .ValueGeneratedOnAdd();

    nBuilder.HasIndex(x => x.Username)
            .IsUnique();
    nBuilder.Property(x => x.Username)
            .HasMaxLength(50);

    nBuilder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

    nBuilder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETDATE()");
    nBuilder.Property(x => x.UpdatedAt)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("GETDATE()");
  }
}
