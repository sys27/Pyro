// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses.Identity;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Identity.Queries;
using Riok.Mapperly.Abstractions;

namespace Pyro.DtoMappings;

[Mapper]
public static partial class IdentityMapper
{
    [MapperIgnoreSource(nameof(Permission.Id))]
    [MapperIgnoreSource(nameof(Permission.Roles))]
    public static partial PermissionResponse ToResponse(this Permission permission);

    public static partial IEnumerable<PermissionResponse> ToResponse(this IEnumerable<Permission> role);

    [MapperIgnoreSource(nameof(Role.Id))]
    [MapperIgnoreSource(nameof(Role.Users))]
    public static partial RoleResponse ToResponse(this Role role);

    public static partial IEnumerable<RoleResponse> ToResponse(this IEnumerable<Role> role);

    [MapperIgnoreSource(nameof(User.Password))]
    [MapperIgnoreSource(nameof(User.Salt))]
    [MapperIgnoreSource(nameof(User.AuthenticationTokens))]
    [MapperIgnoreSource(nameof(User.AccessTokens))]
    [MapperIgnoreSource(nameof(User.OneTimePasswords))]
    [MapperIgnoreSource(nameof(User.DomainEvents))]
    public static partial UserResponse ToResponse(this User user);

    public static partial IReadOnlyList<UserResponse> ToResponse(this IReadOnlyList<User> user);

    public static partial CreateUser ToCommand(this CreateUserRequest user);

    public static partial ActivateUser ToCommand(this ActivateUserRequest request);

    [MapProperty([nameof(JwtTokenPair.AccessToken), nameof(Token.Value)], nameof(TokenPairResponse.AccessToken))]
    [MapProperty([nameof(JwtTokenPair.RefreshToken), nameof(Token.Value)], nameof(TokenPairResponse.RefreshToken))]
    public static partial TokenPairResponse ToResponse(this JwtTokenPair jwtTokenPair);

    [MapperIgnoreSource(nameof(RefreshTokenResult.IsSuccess))]
    public static partial TokenResponse ToResponse(this RefreshTokenResult token);

    public static partial CreateAccessToken ToCommand(this CreateAccessTokenRequest token);

    public static partial CreateAccessTokenResult ToResponse(this CreateAccessTokenResult token);

    [MapperIgnoreSource(nameof(AccessToken.Id))]
    [MapperIgnoreSource(nameof(AccessToken.Token))]
    [MapperIgnoreSource(nameof(AccessToken.Salt))]
    [MapperIgnoreSource(nameof(AccessToken.UserId))]
    public static partial AccessTokenResponse ToResponse(this AccessToken token);

    public static partial IEnumerable<AccessTokenResponse> ToResponse(this IEnumerable<AccessToken> token);

    public static partial GetUsers ToGetUsers(this PageRequest<string> request);
}