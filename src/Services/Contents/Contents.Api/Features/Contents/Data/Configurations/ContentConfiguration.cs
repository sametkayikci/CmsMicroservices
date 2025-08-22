using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Contents.Api.Features.Contents.Data.Configurations;


public sealed class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        b.Property(x => x.UserId).IsRequired();
        b.HasIndex(x => x.UserId);
        b.Property(x => x.CreatedAtUtc).IsRequired();
    }
}