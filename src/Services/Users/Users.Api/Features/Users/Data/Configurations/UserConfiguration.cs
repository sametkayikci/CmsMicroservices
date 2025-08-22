using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Api.Features.Users.Entities;


namespace Users.Api.Features.Users.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        b.Property(x => x.CreatedAtUtc).IsRequired();
    }
}