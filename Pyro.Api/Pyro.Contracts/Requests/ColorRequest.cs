// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Requests;

public record ColorRequest(int R, int G, int B)
{
    public int ToInt()
        => (R << 16) + (G << 8) + B;

    public static ColorRequest FromHex(string hex)
    {
        if (hex.Length != 7)
            throw new ArgumentException("Hex color must be 7 characters long");
        if (hex[0] != '#')
            throw new ArgumentException("Hex color must start with #");

        var r = Convert.ToInt32(hex[1..3], 16);
        var g = Convert.ToInt32(hex[3..5], 16);
        var b = Convert.ToInt32(hex[5..], 16);

        return new ColorRequest(r, g, b);
    }
}