// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories.Commands;

public record CreateGitRepository(
    string Name,
    string? Description,
    string DefaultBranch) : IRequest<GitRepository>;

public class CreateGitRepositoryValidator : AbstractValidator<CreateGitRepository>
{
    public CreateGitRepositoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(Regexes.GetRepositoryNameRegex());

        RuleFor(x => x.Description)
            .MaximumLength(250);

        RuleFor(x => x.DefaultBranch)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class CreateGitRepositoryHandler : IRequestHandler<CreateGitRepository, GitRepository>
{
    private readonly IGitRepositoryRepository repository;
    private readonly IBus bus;

    public CreateGitRepositoryHandler(
        IGitRepositoryRepository repository,
        IBus bus)
    {
        this.repository = repository;
        this.bus = bus;
    }

    public async Task<GitRepository> Handle(
        CreateGitRepository request,
        CancellationToken cancellationToken = default)
    {
        var gitRepository = new GitRepository
        {
            Name = request.Name,
            Description = request.Description,
            DefaultBranch = request.DefaultBranch,
        };
        gitRepository = await repository.AddGitRepository(gitRepository, cancellationToken);

        var gitRepositoryCreated = new GitRepositoryCreated(gitRepository.Id);
        await bus.Publish(gitRepositoryCreated, cancellationToken);

        return gitRepository;
    }
}