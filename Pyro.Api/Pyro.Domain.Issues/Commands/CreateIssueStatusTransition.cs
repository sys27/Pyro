// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record CreateIssueStatusTransition(
    string RepositoryName,
    Guid FromId,
    Guid ToId) : IRequest<IssueStatusTransition>;

public class CreateIssueStatusTransitionValidator : AbstractValidator<CreateIssueStatusTransition>
{
    public CreateIssueStatusTransitionValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.FromId)
            .NotEmpty();

        RuleFor(x => x.ToId)
            .NotEmpty();
    }
}

public class CreateIssueStatusTransitionHandler
    : IRequestHandler<CreateIssueStatusTransition, IssueStatusTransition>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public CreateIssueStatusTransitionHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IssueStatusTransition> Handle(
        CreateIssueStatusTransition request,
        CancellationToken cancellationToken = default)
    {
        if (request.FromId == request.ToId)
            throw new DomainException("The transition status cannot be the same");

        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");
        var status = repository.GetIssueStatus(request.FromId) ??
                     throw new NotFoundException($"The issue status (Id: {request.FromId}) not found");
        var toStatus = repository.GetIssueStatus(request.ToId) ??
                       throw new NotFoundException($"The issue status (Id: {request.ToId}) not found");

        var transition = status.AddTransition(toStatus);

        return transition;
    }
}