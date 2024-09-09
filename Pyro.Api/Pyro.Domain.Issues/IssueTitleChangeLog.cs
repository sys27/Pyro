// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues;

public class IssueTitleChangeLog : IssueChangeLog
{
    private IssueTitleChangeLog()
    {
    }

    public IssueTitleChangeLog(string? oldTitle, string? newTitle)
    {
        OldTitle = oldTitle;
        NewTitle = newTitle;
    }

    public string? OldTitle { get; }

    public string? NewTitle { get; }
}