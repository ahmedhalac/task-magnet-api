using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMagnet.Core.Domain.Entities;

namespace TaskMagnet.Infrastructure.Database;

public class UserConfiguration : BaseEntityConfiguration<User, long>
{
    public override void Configure(EntityTypeBuilder<User> builder) 
    {
        base.Configure(builder);
        builder.ToTable("users");

        builder.Property(x => x.FirstName).HasColumnName("first_name");
        builder.Property(x => x.LastName).HasColumnName("last_name");
        builder.Property(x => x.Username).HasColumnName("username");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash");
        builder.Property(x => x.PasswordSalt).HasColumnName("password_salt");
    }

}
