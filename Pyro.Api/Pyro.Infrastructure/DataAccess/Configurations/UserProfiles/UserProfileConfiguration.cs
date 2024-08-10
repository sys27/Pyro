// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure.DataAccess.Configurations.UserProfiles;

internal class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(x => x.Id)
            .HasName("PK_UserProfile");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasMaxLength(200);

        builder.HasOne(x => x.Avatar)
            .WithOne()
            .HasForeignKey<UserAvatar>(x => x.Id);

        builder.HasData(SeedData.UserProfiles);
    }
}