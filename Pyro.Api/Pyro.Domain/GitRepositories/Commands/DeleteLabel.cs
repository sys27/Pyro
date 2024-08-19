// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.GitRepositories.Commands;

public record DeleteLabel(string RepositoryName, Guid Id) : IRequest;

public class DeleteLabelValidator : AbstractValidator<DeleteLabel>
{
    public DeleteLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteLabelHandler : IRequestHandler<DeleteLabel>
{
    private readonly IGitRepositoryRepository repository;

    public DeleteLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task Handle(DeleteLabel request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken);
        if (gitRepository is null)
            return;

        gitRepository.RemoveLabel(request.Id);
    }
}