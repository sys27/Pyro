// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Contracts.Responses;

public record LabelResponse(Guid Id, string Name, ColorResponse Color, bool IsDisabled);