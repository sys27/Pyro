// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.EntityFrameworkCore;
using Pyro.Domain.GitRepositories;
using Pyro.Infrastructure.Issues.DataAccess;
using Pyro.Infrastructure.Issues.DataAccess.Configurations;

namespace Pyro.DomainEventHandlers;

public class LabelDeletedHandler : INotificationHandler<LabelDeleted>
{
    private readonly IssuesDbContext dbContext;

    public LabelDeletedHandler(IssuesDbContext dbContext)
        => this.dbContext = dbContext;

    public async Task Handle(LabelDeleted notification, CancellationToken cancellationToken)
        => await dbContext
            .Set<IssueLabel>()
            .Where(x => x.LabelId == notification.Id)
            .ExecuteDeleteAsync(cancellationToken);
}