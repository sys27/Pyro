// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Queries;

namespace Pyro.Domain.Issues.Queries;

public record GetIssues(string RepositoryName, int Size, Guid? Before, Guid? After)
    : IRequest<IReadOnlyList<Issue>>, IPageQuery<Guid?>;

public class GetIssuesValidator : AbstractValidator<GetIssues>
{
    public GetIssuesValidator()
    {
        Include(new PageQueryValidator<Guid?>());

        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class GetIssuesHandler : IRequestHandler<GetIssues, IReadOnlyList<Issue>>
{
    private readonly IIssueRepository repository;

    public GetIssuesHandler(IIssueRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<Issue>> Handle(GetIssues request, CancellationToken cancellationToken)
    {
        var issues = await repository.GetIssues(request, cancellationToken);

        return issues;
    }
}