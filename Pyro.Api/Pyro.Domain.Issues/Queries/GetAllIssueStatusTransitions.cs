// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Queries;

public record GetAllIssueStatusTransitions(string RepositoryName) : IRequest<IReadOnlyList<IssueStatusTransition>>;

public class GetAllIssueStatusTransitionsValidator : AbstractValidator<GetAllIssueStatusTransitions>
{
    public GetAllIssueStatusTransitionsValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class GetAllIssueStatusTransitionsHandler
    : IRequestHandler<GetAllIssueStatusTransitions, IReadOnlyList<IssueStatusTransition>>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public GetAllIssueStatusTransitionsHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IReadOnlyList<IssueStatusTransition>> Handle(
        GetAllIssueStatusTransitions request,
        CancellationToken cancellationToken = default)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        return repository.GetTransitions();
    }
}