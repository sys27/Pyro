// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.GitRepositories.Commands;

public record UpdateTag(string RepositoryName, Guid Id, string NewName, int NewColor) : IRequest<Tag>;

public class UpdateTagValidator : AbstractValidator<UpdateTag>
{
    public UpdateTagValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NewName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.NewColor)
            .NotEmpty()
            .InclusiveBetween(0, 0xFFFFFF);
    }
}

public class UpdateTagHandler : IRequestHandler<UpdateTag, Tag>
{
    private readonly IGitRepositoryRepository repository;

    public UpdateTagHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<Tag> Handle(UpdateTag request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken) ??
                            throw new NotFoundException($"The '{request.RepositoryName}' repository not found");

        var tag = gitRepository.GetTag(request.Id) ??
                  throw new NotFoundException($"The tag (Id: {request.Id}) not found");

        tag.Name = request.NewName;
        tag.Color = request.NewColor;

        return tag;
    }
}