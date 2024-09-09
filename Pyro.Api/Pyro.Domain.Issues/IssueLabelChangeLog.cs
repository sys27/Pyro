// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Issues;

public class IssueLabelChangeLog : IssueChangeLog
{
    private IssueLabelChangeLog()
    {
    }

    public IssueLabelChangeLog(Label? oldLabel, Label? newLabel)
    {
        if (oldLabel is not null && newLabel is not null)
            throw new DomainException("Old and new labels cannot be set at the same time");

        OldLabel = oldLabel;
        NewLabel = newLabel;
    }

    public Label? OldLabel { get; }

    public Label? NewLabel { get; }
}