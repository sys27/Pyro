// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pyro.Infrastructure.Shared.DataAccess.Converters;

public class GuidConverter : ValueConverter<Guid, byte[]>
{
    public static readonly GuidConverter Instance = new GuidConverter();

    public GuidConverter()
        : base(x => x.ToByteArray(), x => new Guid(x))
    {
    }
}