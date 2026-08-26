using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wms.Infrastructure.Authentication;

namespace Wms.Infrastructure.Persistence.Configurations.Authentication;

public sealed class RefreshSessionConfiguration
    : IEntityTypeConfiguration<RefreshSession>
{
    public void Configure(EntityTypeBuilder<RefreshSession> builder)
    {
        builder.ToTable("refresh_sessions", "auth");

        builder.HasKey(session => session.Id);

        builder
            .Property(session => session.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(session => session.CreatedByIp)
            .HasMaxLength(64);

        builder
            .Property(session => session.RevokedByIp)
            .HasMaxLength(64);

        builder
            .Property(session => session.UserAgent)
            .HasMaxLength(512);

        builder
            .Property(session => session.RevocationReason)
            .HasMaxLength(80);

        builder
            .HasIndex(session => session.TokenHash)
            .IsUnique()
            .HasDatabaseName("ux_refresh_sessions_token_hash");

        builder
            .HasIndex(session => new
            {
                session.UserId,
                session.ExpiresAtUtc
            })
            .HasDatabaseName("ix_refresh_sessions_user_expiration");

        builder
            .HasOne(session => session.User)
            .WithMany()
            .HasForeignKey(session => session.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(session => session.ReplacedBySession)
            .WithMany()
            .HasForeignKey(session => session.ReplacedBySessionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
