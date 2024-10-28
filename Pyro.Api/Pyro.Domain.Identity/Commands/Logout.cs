// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using MediatR;
using Microsoft.Extensions.Logging;
using Pyro.Domain.Shared.CurrentUserProvider;

namespace Pyro.Domain.Identity.Commands;

public record Logout : IRequest;

public class LogoutHandler : IRequestHandler<Logout>
{
    private readonly ILogger<LogoutHandler> logger;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository userRepository;

    public LogoutHandler(
        ILogger<LogoutHandler> logger,
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository)
    {
        this.logger = logger;
        this.currentUserProvider = currentUserProvider;
        this.userRepository = userRepository;
    }

    public async Task Handle(Logout request, CancellationToken cancellationToken = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await userRepository.GetUserById(currentUser.Id, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("User with ID {UserId} was not found", currentUser.Id);

            return;
        }

        // TODO: for now we just remove all refresh tokens
        user.ClearAuthenticationTokens();
    }
}