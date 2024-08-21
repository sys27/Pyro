// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Queries;

public record GetIssueStatus(string RepositoryName, Guid Id) : IRequest<IssueStatus?>;

public class GetIssueStatusValidator : AbstractValidator<GetIssueStatus>
{
    public GetIssueStatusValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetIssueStatusHandler : IRequestHandler<GetIssueStatus, IssueStatus?>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public GetIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IssueStatus?> Handle(GetIssueStatus request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        return repository.GetIssueStatus(request.Id);
    }
}