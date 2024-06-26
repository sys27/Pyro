// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Git;

namespace Pyro.Domain.GitRepositories.Queries;

// TODO: add validator
public record GetTreeView(string RepositoryName, string? BranchOrHash, string? Path) : IRequest<TreeView?>;

public class GetTreeViewHandler : IRequestHandler<GetTreeView, TreeView?>
{
    private readonly IGitRepositoryRepository gitRepository;
    private readonly IGitService gitService;

    public GetTreeViewHandler(
        IGitRepositoryRepository gitRepository,
        IGitService gitService)
    {
        this.gitRepository = gitRepository;
        this.gitService = gitService;
    }

    public async Task<TreeView?> Handle(GetTreeView request, CancellationToken cancellationToken)
    {
        var repository = await gitRepository.GetGitRepository(request.RepositoryName, cancellationToken);
        if (repository is null)
            return null;

        var treeView = gitService.GetTreeView(repository, request.BranchOrHash, request.Path);

        return treeView;
    }
}