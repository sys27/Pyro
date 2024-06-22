// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Logging;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.GitRepositories;

public record GitRepositoryCreated(Guid GitRepositoryId) : IntegrationEvent;

public class GitRepositoryCreatedHandler : INotificationHandler<GitRepositoryCreated>
{
    private readonly ILogger<GitRepositoryCreatedHandler> logger;
    private readonly IGitRepositoryRepository repository;
    private readonly IGitService git;

    public GitRepositoryCreatedHandler(
        ILogger<GitRepositoryCreatedHandler> logger,
        IGitRepositoryRepository repository,
        IGitService git)
    {
        this.logger = logger;
        this.repository = repository;
        this.git = git;
    }

    public async Task Handle(GitRepositoryCreated notification, CancellationToken cancellationToken)
    {
        var gitRepository = await repository.GetGitRepository(notification.GitRepositoryId, cancellationToken);
        if (gitRepository is null)
        {
            logger.LogWarning("Git repository {GitRepositoryId} not found", notification.GitRepositoryId);
            return;
        }

        await git.InitializeRepository(gitRepository, cancellationToken);
    }
}