// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetLabels(string RepositoryName, string? LabelName) : IRequest<IReadOnlyList<Label>>;

public class GetLabelsValidator : AbstractValidator<GetLabels>
{
    public GetLabelsValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class GetLabelsHandler : IRequestHandler<GetLabels, IReadOnlyList<Label>>
{
    private readonly IGitRepositoryRepository repository;

    public GetLabelsHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<Label>> Handle(GetLabels request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        return gitRepository.Labels
            .Where(x => request.LabelName == null || x.Name.Contains(request.LabelName))
            .ToList();
    }
}