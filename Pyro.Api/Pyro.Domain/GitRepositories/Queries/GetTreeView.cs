// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Git;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetTreeView(string RepositoryName, string? BranchOrPath) : IRequest<TreeView?>;

public class GetTreeViewValidator : AbstractValidator<GetTreeView>
{
    public GetTreeViewValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(Regexes.GetRepositoryNameRegex());
    }
}

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

        var treeView = gitService.GetTreeView(repository, request.BranchOrPath);

        return treeView;
    }
}