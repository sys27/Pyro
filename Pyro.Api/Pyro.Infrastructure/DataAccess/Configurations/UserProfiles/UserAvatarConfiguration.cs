// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure.DataAccess.Configurations.UserProfiles;

internal class UserAvatarConfiguration : IEntityTypeConfiguration<UserAvatar>
{
    public void Configure(EntityTypeBuilder<UserAvatar> builder)
    {
        builder.ToTable("UserAvatars");

        builder.HasKey(x => x.Id)
            .HasName("PK_UserAvatar");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Image)
            .IsRequired();
    }
}