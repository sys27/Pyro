// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Core;

namespace Pyro.Domain.GitRepositories;

public record CreateGitRepository(string Name) : IRequest<GitRepository>;

public class CreateGitRepositoryValidator : AbstractValidator<CreateGitRepository>
{
    public CreateGitRepositoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(20)
            .Matches(@"^[a-zA-Z0-9\\-\\_]+$");
    }
}

public class CreateGitRepositoryHandler : IRequestHandler<CreateGitRepository, GitRepository>
{
    private readonly IGitRepositoryRepository repository;
    private readonly IBus bus;

    public CreateGitRepositoryHandler(IGitRepositoryRepository repository, IBus bus)
    {
        this.repository = repository;
        this.bus = bus;
    }

    public async Task<GitRepository> Handle(CreateGitRepository request, CancellationToken cancellationToken)
    {
        var gitRepository = new GitRepository
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };
        gitRepository = await repository.AddGitRepository(gitRepository, cancellationToken);

        var gitRepositoryCreated = new GitRepositoryCreated(gitRepository.Id);
        await bus.Publish(gitRepositoryCreated, cancellationToken);

        return gitRepository;
    }
}