// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Logging;
using Pyro.Domain.Core.Exceptions;
using Pyro.Domain.Core.Models;

namespace Pyro.Domain.GitRepositories;

public record GitRepositoryNameChanged(string? OldName, string NewName) : IDomainEvent;

public class GitRepositoryNameChangedHandler : INotificationHandler<GitRepositoryNameChanged>
{
    private readonly ILogger<GitRepositoryNameChangedHandler> logger;
    private readonly IGitRepositoryRepository repository;

    public GitRepositoryNameChangedHandler(
        ILogger<GitRepositoryNameChangedHandler> logger,
        IGitRepositoryRepository repository)
    {
        this.logger = logger;
        this.repository = repository;
    }

    public async Task Handle(GitRepositoryNameChanged notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling domain event {DomainEvent}. The name was changed from {OldName} to {NewName}",
            nameof(GitRepositoryNameChanged),
            notification.OldName,
            notification.NewName);

        var gitRepository = await repository.GetGitRepository(notification.NewName, cancellationToken);
        if (gitRepository is not null)
        {
            throw new DomainException($"Git repository with name '{notification.NewName}' already exists.");
        }
    }
}