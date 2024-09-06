// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Queries;

public record GetIssueStatuses(string RepositoryName, string? StatusName) : IRequest<IReadOnlyList<IssueStatus>>;

public class GetIssueStatusesValidator : AbstractValidator<GetIssueStatuses>
{
    public GetIssueStatusesValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.StatusName)
            .MaximumLength(50);
    }
}

public class GetIssueStatusesHandler : IRequestHandler<GetIssueStatuses, IReadOnlyList<IssueStatus>>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public GetIssueStatusesHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IReadOnlyList<IssueStatus>> Handle(
        GetIssueStatuses request,
        CancellationToken cancellationToken = default)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        return repository.IssueStatuses
            .Where(x => request.StatusName == null || x.Name.Contains(request.StatusName))
            .ToList();
    }
}