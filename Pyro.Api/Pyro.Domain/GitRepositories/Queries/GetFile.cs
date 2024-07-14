// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Git;
using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.GitRepositories.Queries;

public record GetFile(string RepositoryName, string Path) : IRequest<GitFile?>;

public class GetFileValidator : AbstractValidator<GetFile>
{
    public GetFileValidator()
    {
        RuleFor(x => x.RepositoryName)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(Regexes.GetRepositoryNameRegex());

        RuleFor(x => x.Path)
            .NotEmpty();
    }
}

public class GetFileHandler : IRequestHandler<GetFile, GitFile?>
{
    private readonly IGitRepositoryRepository gitRepository;
    private readonly IGitService gitService;

    public GetFileHandler(
        IGitRepositoryRepository gitRepository,
        IGitService gitService)
    {
        this.gitRepository = gitRepository;
        this.gitService = gitService;
    }

    public async Task<GitFile?> Handle(GetFile request, CancellationToken cancellationToken)
    {
        var repository = await gitRepository.GetGitRepository(request.RepositoryName, cancellationToken);
        if (repository is null)
            return null;

        var file = gitService.GetFile(repository, request.Path);

        return file;
    }
}