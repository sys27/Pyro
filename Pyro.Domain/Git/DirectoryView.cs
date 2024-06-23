// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Git;

public record DirectoryView(
    CommitInfo Commit,
    IReadOnlyList<DirectoryViewItem> Items,
    int CommitsCount);