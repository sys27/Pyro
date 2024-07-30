// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pyro.Infrastructure.Shared.DataAccess.Converters;

internal class ReadOnlyListToByteArrayConverter<T> : ValueConverter<IReadOnlyList<T>, T[]>
{
    public ReadOnlyListToByteArrayConverter()
        : base(x => x.ToArray(), x => x)
    {
    }
}