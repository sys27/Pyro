// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues;

public class IssueStatusChangeLog : IssueChangeLog
{
    private IssueStatusChangeLog()
    {
    }

    public IssueStatusChangeLog(IssueStatus? oldStatus, IssueStatus? newStatus)
    {
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }

    public IssueStatus? OldStatus { get; }

    public IssueStatus? NewStatus { get; }
}