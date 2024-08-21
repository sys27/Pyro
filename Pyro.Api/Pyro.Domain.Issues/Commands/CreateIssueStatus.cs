// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record CreateIssueStatus(string RepositoryName, string Name, int Color) : IRequest<IssueStatus>;

public class CreateIssueStatusValidator : AbstractValidator<CreateIssueStatus>
{
    public CreateIssueStatusValidator()
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

public class CreateIssueStatusHandler : IRequestHandler<CreateIssueStatus, IssueStatus>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public CreateIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IssueStatus> Handle(
        CreateIssueStatus request,
        CancellationToken cancellationToken = default)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        var status = repository.AddIssueStatus(request.Name, request.Color);

        return status;
    }
}