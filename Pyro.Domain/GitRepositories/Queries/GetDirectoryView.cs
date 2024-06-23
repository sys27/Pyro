// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Git;

namespace Pyro.Domain.GitRepositories.Queries;

// TODO: add validator
public record GetDirectoryView(string Name) : IRequest<DirectoryView?>;

public class GetDirectoryViewHandler : IRequestHandler<GetDirectoryView, DirectoryView?>
{
    private readonly IGitRepositoryRepository gitRepository;
    private readonly IGitService gitService;

    public GetDirectoryViewHandler(
        IGitRepositoryRepository gitRepository,
        IGitService gitService)
    {
        this.gitRepository = gitRepository;
        this.gitService = gitService;
    }

    public async Task<DirectoryView?> Handle(GetDirectoryView request, CancellationToken cancellationToken)
    {
        var repository = await gitRepository.GetGitRepository(request.Name, cancellationToken);
        if (repository is null)
            return null;

        var directoryView = gitService.GetDirectoryView(repository);

        return directoryView;
    }
}