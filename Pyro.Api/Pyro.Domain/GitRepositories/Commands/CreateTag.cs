// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record CreateTag(string RepositoryName, string Name, int Color) : IRequest<Tag>;

public class CreateTagValidator : AbstractValidator<CreateTag>
{
    public CreateTagValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Color)
            .NotEmpty()
            .InclusiveBetween(0, 0xFFFFFF);
    }
}

public class CreateTagHandler : IRequestHandler<CreateTag, Tag>
{
    private readonly IGitRepositoryRepository repository;

    public CreateTagHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Tag> Handle(CreateTag request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var tag = gitRepository.AddTag(request.Name, request.Color);

        return tag;
    }
}