// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pyro.Infrastructure.Shared.DataAccess.Converters;

public class NullableGuidConverter : ValueConverter<Guid?, byte[]?>
{
    public static readonly NullableGuidConverter Instance = new NullableGuidConverter();

    public NullableGuidConverter()
        : base(
            x => x != null ? x.Value.ToByteArray() : null,
            x => x != null ? new Guid(x) : null)
    {
    }
}