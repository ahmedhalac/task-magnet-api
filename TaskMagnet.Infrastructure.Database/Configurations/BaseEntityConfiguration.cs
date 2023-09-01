using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskMagnet.Core.Domain.Interfaces;

namespace TaskMagnet.Infrastructure.Database;

public class BaseEntityConfiguration<T, U> : IEntityTypeConfiguration<T> where T : class, IBaseEntity<U>
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.CreatedById).HasColumnName("created_by_id");
        builder.Property(x => x.ModifiedById).HasColumnName("modified_by_id");
        builder.Property(x => x.CreatedDate).HasColumnName("created_date").HasDefaultValueSql("getutcdate()");
        builder.Property(x => x.ModifiedDate).HasColumnName("modified_date").HasDefaultValueSql("getutcdate()");
    }
}
