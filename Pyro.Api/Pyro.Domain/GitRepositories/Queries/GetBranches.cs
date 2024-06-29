// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Git;

namespace Pyro.Domain.GitRepositories.Queries;

// TODO: validator
public record GetBranches(string RepositoryName) : IRequest<IReadOnlyList<BranchItem>>;

public class GetBranchesHandler : IRequestHandler<GetBranches, IReadOnlyList<BranchItem>>
{
    private readonly IGitRepositoryRepository gitRepository;
    private readonly IGitService gitService;

    public GetBranchesHandler(
        IGitRepositoryRepository gitRepository,
        IGitService gitService)
    {
        this.gitRepository = gitRepository;
        this.gitService = gitService;
    }

    public async Task<IReadOnlyList<BranchItem>> Handle(GetBranches request, CancellationToken cancellationToken)
    {
        var repository = await gitRepository.GetGitRepository(request.RepositoryName, cancellationToken);
        if (repository is null)
            return [];

        var branches = gitService.GetBranches(repository);

        return branches;
    }
}