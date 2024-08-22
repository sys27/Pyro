// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record CreateLabel(string RepositoryName, string Name, int Color) : IRequest<Label>;

public class CreateLabelValidator : AbstractValidator<CreateLabel>
{
    public CreateLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .InclusiveBetween(0, 0xFFFFFF);
    }
}

public class CreateLabelHandler : IRequestHandler<CreateLabel, Label>
{
    private readonly IGitRepositoryRepository repository;

    public CreateLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Label> Handle(CreateLabel request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var label = gitRepository.AddLabel(request.Name, request.Color);

        return label;
    }
}
