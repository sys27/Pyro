// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues;

public class IssueLockChangeLog : IssueChangeLog
{
    private IssueLockChangeLog()
    {
    }

    public IssueLockChangeLog(bool oldValue, bool newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public bool OldValue { get; init; }

    public bool NewValue { get; init; }
}