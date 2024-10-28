// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.Identity.DataAccess.Configurations;

internal class OneTimePasswordConfiguration : IEntityTypeConfiguration<OneTimePassword>
{
    public void Configure(EntityTypeBuilder<OneTimePassword> builder)
    {
        builder.ToTable("OneTimePasswords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.ExpiresAt)
            .IsRequired()
            .HasConversion(new DateTimeOffsetToBinaryConverter());

        builder.Property(x => x.Purpose)
            .IsRequired();

        builder.Property<Guid>("UserId")
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.OneTimePasswords)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_OneTimePasswords_Users_UserId");

        builder.HasIndex(x => x.Token)
            .IsUnique()
            .HasDatabaseName("IX_OneTimePasswords_Token");
    }
}