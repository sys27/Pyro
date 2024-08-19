// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetLabel(string RepositoryName, Guid Id) : IRequest<Label>;

public class GetLabelValidator : AbstractValidator<GetLabel>
{
    public GetLabelValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetLabelHandler : IRequestHandler<GetLabel, Label>
{
    private readonly IGitRepositoryRepository repository;

    public GetLabelHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Label> Handle(GetLabel request, CancellationToken cancellationToken)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The repository (Name: {request.RepositoryName}) was not found.");

        var label = gitRepository.GetLabel(request.Id) ??
                    throw new NotFoundException($"The label (Id: {request.Id}) was not found.");

        return label;
    }
}