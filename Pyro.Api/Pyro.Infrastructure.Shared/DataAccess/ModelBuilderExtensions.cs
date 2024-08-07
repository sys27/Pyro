// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pyro.Infrastructure.Shared.DataAccess;

public static class ModelBuilderExtensions
{
    public static void ConfigureIds(this ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(x => x.ClrType.GetInterface("IEntity") is not null);

        foreach (var entity in entities)
        {
            var idProperty = entity.FindProperty("Id");
            if (idProperty is not null)
                idProperty.ValueGenerated = ValueGenerated.Never;
        }
    }
}