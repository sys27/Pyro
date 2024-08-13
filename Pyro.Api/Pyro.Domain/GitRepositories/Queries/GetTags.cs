// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetTags(string RepositoryName) : IRequest<IReadOnlyList<Tag>>;

public class GetTagsValidator : AbstractValidator<GetTags>
{
    public GetTagsValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public class GetTagsHandler : IRequestHandler<GetTags, IReadOnlyList<Tag>>
{
    private readonly IGitRepositoryRepository repository;

    public GetTagsHandler(IGitRepositoryRepository repository)
        => this.repository = repository;

    public async Task<IReadOnlyList<Tag>> Handle(GetTags request, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(request.RepositoryName, cancellationToken);

        return gitRepository?.Tags ?? [];
    }
}