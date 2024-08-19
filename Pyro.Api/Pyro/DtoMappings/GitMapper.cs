// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests;
using Pyro.Contracts.Responses;
using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.GitRepositories.Commands;
using Pyro.Domain.GitRepositories.Queries;
using Riok.Mapperly.Abstractions;

namespace Pyro.DtoMappings;

[Mapper]
public static partial class GitMapper
{
    [MapperIgnoreSource(nameof(GitRepository.Id))]
    [MapperIgnoreSource(nameof(GitRepository.DomainEvents))]
    [MapperIgnoreSource(nameof(GitRepository.IsNew))]
    [MapperIgnoreSource(nameof(GitRepository.Labels))]
    public static partial GitRepositoryResponse ToResponse(this GitRepository gitRepository);

    public static partial GitRepositoryStatusResponse ToResponse(this GitRepositoryStatus gitRepository);

    public static partial IReadOnlyList<GitRepositoryResponse> ToResponse(this IReadOnlyList<GitRepository> gitRepository);

    public static partial CreateGitRepository ToCommand(this CreateGitRepositoryRequest request);

    public static partial CommitUserResponse ToResponse(this CommitUser user);

    public static partial CommitInfoResponse ToResponse(this CommitInfo info);

    public static partial TreeViewItemResponse ToResponse(this TreeViewItem user);

    public static partial TreeViewResponse ToResponse(this TreeView user);

    public static partial BranchItemResponse ToResponse(this BranchItem user);

    public static partial IReadOnlyList<BranchItemResponse> ToResponse(this IReadOnlyList<BranchItem> user);

    public static partial GetGitRepositories ToGetGitRepositories(this PageRequest<string> request);

    [UserMapping(Default = true)]
    public static ColorResponse ToResponse(this int color)
    {
        var r = (byte)((color & 0xFF0000) >> 16);
        var g = (byte)((color & 0x00FF00) >> 8);
        var b = (byte)(color & 0x0000FF);

        return new ColorResponse(r, g, b);
    }

    [MapperIgnoreSource(nameof(Label.GitRepository))]
    public static partial LabelResponse ToResponse(this Label request);

    public static partial IReadOnlyList<LabelResponse> ToResponse(this IReadOnlyList<Label> request);
}