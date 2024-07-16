// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.GitRepositories.Commands;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Riok.Mapperly.Abstractions;

namespace Pyro.Dtos.Mapping;

[Mapper]
public static partial class GitMapper
{
    [MapperIgnoreSource(nameof(GitRepository.Id))]
    [MapperIgnoreSource(nameof(GitRepository.DomainEvents))]
    [MapperIgnoreSource(nameof(GitRepository.IsNew))]
    public static partial GitRepositoryResponse ToResponse(this GitRepository gitRepository);

    public static partial IReadOnlyList<GitRepositoryResponse> ToResponse(this IReadOnlyList<GitRepository> gitRepository);

    public static partial CreateGitRepository ToCommand(this CreateGitRepositoryRequest request);

    public static partial CommitUserResponse ToResponse(this CommitUser user);

    public static partial CommitInfoResponse ToResponse(this CommitInfo info);

    public static partial TreeViewItemResponse ToResponse(this TreeViewItem user);

    public static partial TreeViewResponse ToResponse(this TreeView user);

    public static partial BranchItemResponse ToResponse(this BranchItem user);

    public static partial IReadOnlyList<BranchItemResponse> ToResponse(this IReadOnlyList<BranchItem> user);
}