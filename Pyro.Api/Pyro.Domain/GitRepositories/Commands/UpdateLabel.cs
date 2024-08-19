// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record UpdateLabel(string RepositoryName, Guid Id, string NewName, int NewColor) : IRequest<Label>;

public class UpdateLabelValidator : AbstractValidator<UpdateLabel>
{
    public UpdateLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NewColor)
            .NotEmpty()
            .InclusiveBetween(0, 0xFFFFFF);
    }
}

public class UpdateLabelHandler : IRequestHandler<UpdateLabel, Label>
{
    private readonly IGitRepositoryRepository repository;

    public UpdateLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Label> Handle(UpdateLabel request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var label = gitRepository.GetLabel(request.Id) ??
                    throw new NotFoundException($"The label (Id: {request.Id}) not found");

        label.Name = request.NewName;
        label.Color = request.NewColor;

        return label;
    }
}