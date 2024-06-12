// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;

namespace Pyro.Domain.GitRepositories;

public record GetGitRepositories : IRequest<IReadOnlyList<GitRepository>>;

public class GetGitRepositoriesHandler : IRequestHandler<GetGitRepositories, IReadOnlyList<GitRepository>>
{
    private readonly IGitRepositoryRepository repository;

    public GetGitRepositoriesHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public Task<IReadOnlyList<GitRepository>> Handle(GetGitRepositories request, CancellationToken cancellationToken)
    {
        var repositories = repository.GetRepositories(cancellationToken);

        return repositories;
    }
}