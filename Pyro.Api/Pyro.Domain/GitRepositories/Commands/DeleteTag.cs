// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.GitRepositories.Commands;

public record DeleteTag(string RepositoryName, Guid Id) : IRequest;

public class DeleteTagValidator : AbstractValidator<DeleteTag>
{
    public DeleteTagValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class DeleteTagHandler : IRequestHandler<DeleteTag>
{
    private readonly IGitRepositoryRepository repository;

    public DeleteTagHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task Handle(DeleteTag request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken);
        if (gitRepository is null)
            return;

        gitRepository.RemoveTag(request.Id);
    }
}