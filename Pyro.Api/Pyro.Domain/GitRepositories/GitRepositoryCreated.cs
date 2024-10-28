// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Messaging;

namespace Pyro.Domain.GitRepositories;

public record GitRepositoryCreated(Guid GitRepositoryId) : IntegrationEvent;