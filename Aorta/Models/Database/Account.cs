using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aorta.Models.Database;
public class Account 
{
  public int ID { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public string Email { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
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
