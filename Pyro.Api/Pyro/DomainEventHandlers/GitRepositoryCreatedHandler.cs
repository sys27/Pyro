// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared;
using Pyro.Infrastructure.DataAccess;

namespace Pyro.DomainEventHandlers;

public class GitRepositoryCreatedHandler : INotificationHandler<GitRepositoryCreated>
{
    private readonly ILogger<GitRepositoryCreatedHandler> logger;
    private readonly PyroDbContext dbContext;
    private readonly IGitRepositoryRepository repository;
    private readonly IGitService git;
    private readonly INotificationService notificationService;

    public GitRepositoryCreatedHandler(
        ILogger<GitRepositoryCreatedHandler> logger,
        PyroDbContext dbContext,
        IGitRepositoryRepository repository,
        IGitService git,
        INotificationService notificationService)
    {
        this.logger = logger;
        this.dbContext = dbContext;
        this.repository = repository;
        this.git = git;
        this.notificationService = notificationService;
    }

    public async Task Handle(GitRepositoryCreated notification, CancellationToken cancellationToken)
    {
        var gitRepository = await repository.GetGitRepository(notification.GitRepositoryId, cancellationToken);
        if (gitRepository is null)
        {
            logger.LogWarning("Git repository {GitRepositoryId} not found", notification.GitRepositoryId);
            return;
        }

        if (!gitRepository.IsNew)
            return;

        await git.InitializeRepository(gitRepository, cancellationToken);

        gitRepository.MarkAsInitialized();
        await dbContext.SaveChangesAsync(cancellationToken);

        await notificationService.RepositoryInitialized(gitRepository.Name);
    }
}