// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetGitRepository(string Name) : IRequest<GitRepository?>;

public class GetGitRepositoryValidator : AbstractValidator<GetGitRepository>
{
    public GetGitRepositoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(Regexes.GetRepositoryNameRegex());
    }
}

public class GetGitRepositoryHandler : IRequestHandler<GetGitRepository, GitRepository?>
{
    private readonly IGitRepositoryRepository repositoryRepository;

    public GetGitRepositoryHandler(IGitRepositoryRepository repositoryRepository)
        => this.repositoryRepository = repositoryRepository;

    public Task<GitRepository?> Handle(GetGitRepository request, CancellationToken cancellationToken)
    {
        var gitRepository = repositoryRepository.GetGitRepository(request.Name, cancellationToken);

        return gitRepository;
    }
}