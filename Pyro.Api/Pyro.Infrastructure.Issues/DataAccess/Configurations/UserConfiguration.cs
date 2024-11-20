// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", b => b.ExcludeFromMigrations());

        builder.HasKey(x => x.Id)
            .HasName("PK_User");

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(50);
    }
}