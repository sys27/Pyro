// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetGitRepositories(int Size, string? Before, string? After)
    : IRequest<IReadOnlyList<GitRepository>>, IPageQuery<string>;

public class GetGitRepositoriesValidator : AbstractValidator<GetGitRepositories>
{
    public GetGitRepositoriesValidator()
    {
        Include(new PageQueryValidator<string>());
    }
}

public class GetGitRepositoriesHandler : IRequestHandler<GetGitRepositories, IReadOnlyList<GitRepository>>
{
    private readonly IGitRepositoryRepository repository;

    public GetGitRepositoriesHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<GitRepository>> Handle(
        GetGitRepositories request,
        CancellationToken cancellationToken = default)
    {
        var repositories = await repository.GetRepositories(request, cancellationToken);

        return repositories;
    }
}