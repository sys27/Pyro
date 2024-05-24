// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Riok.Mapperly.Abstractions;

namespace Pyro.Dtos;

[Mapper]
public static partial class DtoMapper
{
    [MapperIgnoreSource(nameof(GitRepository.Id))]
    [MapperIgnoreSource(nameof(GitRepository.DomainEvents))]
    public static partial GitRepositoryResponse ToResponse(this GitRepository gitRepository);

    public static partial CreateGitRepository ToCommand(this CreateGitRepositoryRequest request);

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
    [MapperIgnoreSource(nameof(User.Tokens))]
    public static partial UserResponse ToResponse(this User user);

    [MapProperty([nameof(TokenPair.AccessToken), nameof(Token.Value)], [nameof(TokenPairResponse.AccessToken)])]
    [MapProperty([nameof(TokenPair.RefreshToken), nameof(Token.Value)], [nameof(TokenPairResponse.RefreshToken)])]
    public static partial TokenPairResponse ToResponse(this TokenPair tokenPair);

    [MapperIgnoreSource(nameof(RefreshTokenResult.IsSuccess))]
    public static partial TokenResponse ToResponse(this RefreshTokenResult token);
}