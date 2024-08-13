// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetTag(string RepositoryName, Guid Id) : IRequest<Tag>;

public class GetTagValidator : AbstractValidator<GetTag>
{
    public GetTagValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}

public class GetTagHandler : IRequestHandler<GetTag, Tag>
{
    private readonly IGitRepositoryRepository repository;

    public GetTagHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Tag> Handle(GetTag request, CancellationToken cancellationToken)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The repository (Name: {request.RepositoryName}) was not found.");

        var tag = gitRepository.GetTag(request.Id) ??
                  throw new NotFoundException($"The tag (Id: {request.Id}) was not found.");

        return tag;
    }
}