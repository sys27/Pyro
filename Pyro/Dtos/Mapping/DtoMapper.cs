// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;
using Pyro.Domain.GitRepositories.Commands;
using Pyro.Domain.UserProfiles;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Riok.Mapperly.Abstractions;

namespace Pyro.Dtos.Mapping;

[Mapper]
public static partial class DtoMapper
{
    [MapperIgnoreSource(nameof(GitRepository.Id))]
    [MapperIgnoreSource(nameof(GitRepository.DomainEvents))]
    public static partial GitRepositoryResponse ToResponse(this GitRepository gitRepository);

    public static partial IReadOnlyList<GitRepositoryResponse> ToResponse(this IReadOnlyList<GitRepository> gitRepository);

    public static partial CreateGitRepository ToCommand(this CreateGitRepositoryRequest request);

    [MapperIgnoreSource(nameof(UserProfile.Id))]
    [MapperIgnoreSource(nameof(UserProfile.Avatar))]
    public static partial UserProfileResponse ToResponse(this UserProfile request);

    public static partial UpdateProfile ToCommand(this UpdateUserProfileRequest request);
}