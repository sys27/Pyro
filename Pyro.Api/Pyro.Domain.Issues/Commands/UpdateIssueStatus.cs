// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues.Commands;

public record UpdateIssueStatus(string RepositoryName, Guid Id, string Name, int Color) : IRequest<IssueStatus>;

public class UpdateIssueStatusValidator : AbstractValidator<UpdateIssueStatus>
{
    public UpdateIssueStatusValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .InclusiveBetween(0, 0xFFFFFF);
    }
}

public class UpdateIssueStatusHandler : IRequestHandler<UpdateIssueStatus, IssueStatus>
{
    private readonly IGitRepositoryRepository gitRepositoryRepository;

    public UpdateIssueStatusHandler(IGitRepositoryRepository gitRepositoryRepository)
        => this.gitRepositoryRepository = gitRepositoryRepository;

    public async Task<IssueStatus> Handle(UpdateIssueStatus request, CancellationToken cancellationToken)
    {
        var repository = await gitRepositoryRepository.GetRepository(request.RepositoryName, cancellationToken) ??
                         throw new NotFoundException($"The repository (Name: {request.RepositoryName}) not found");

        var status = repository.GetIssueStatus(request.Id) ??
                     throw new NotFoundException($"The status (Id: {request.Id}) not found");

        status.Name = request.Name;
        status.Color = request.Color;

        return status;
    }
}