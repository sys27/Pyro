// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pyro.Infrastructure.Messaging;

namespace Pyro.Infrastructure.DataAccess.Configurations.Core;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(x => x.Id)
            .HasName("PK_OutboxMessages");

        builder.Property(x => x.Id)
            .HasColumnType("BLOB")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Message)
            .IsRequired();

        builder.Property(x => x.Retries)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_OutboxMessages_CreatedAt");
    }
}