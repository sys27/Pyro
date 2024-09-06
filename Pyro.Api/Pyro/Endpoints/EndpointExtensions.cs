// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Services;

namespace Pyro.Endpoints;

internal static class EndpointExtensions
{
    public static TBuilder RequirePermission<TBuilder>(
        this TBuilder builder,
        string permission)
        where TBuilder : IEndpointConventionBuilder
        => builder.RequireAuthorization($"{PermissionPolicyProvider.PolicyPrefix}{permission}");
}