// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;

namespace Pyro.ApiTests.Clients;

internal class IdentityClient : BaseClient
{
    public IdentityClient()
    {
    }

    public IdentityClient(Uri baseAddress)
        : base(baseAddress)
    {
    }

    public async Task<IReadOnlyList<UserResponse>?> GetUsers()
        => await Get<IReadOnlyList<UserResponse>>("/api/users?size=50");

    public async Task<UserResponse?> GetUser(string login)
        => await Get<UserResponse>($"/api/users/{login}");

    public async Task CreateUser(CreateUserRequest request)
        => await Post("/api/users", request);

    public async Task<UserResponse?> UpdateUser(string login, UpdateUserRequest request)
        => await Put<UserResponse>($"/api/users/{login}", request);

    public async Task<IReadOnlyList<AccessTokenResponse>?> GetAccessTokens()
        => await Get<IReadOnlyList<AccessTokenResponse>>("/api/users/access-tokens");

    public async Task<AccessTokenResponse?> CreateAccessToken(CreateAccessTokenRequest request)
        => await Post<AccessTokenResponse>("/api/users/access-tokens", request);

    public async Task DeleteAccessToken(string tokenName)
        => await Delete($"/api/users/access-tokens/{tokenName}");

    public async Task<IReadOnlyList<RoleResponse>?> GetRoles()
        => await Get<IReadOnlyList<RoleResponse>>("/api/roles");

    public async Task<IReadOnlyList<PermissionResponse>?> GetPermissions()
        => await Get<IReadOnlyList<PermissionResponse>>("/api/permissions");
}