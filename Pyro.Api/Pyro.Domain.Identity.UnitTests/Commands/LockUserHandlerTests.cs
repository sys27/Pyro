// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.UnitTests.Commands;

public class LockUserHandlerTests
{
    [Test]
    public void LockMissingUser()
    {
        var command = new LockUser("xxx");

        var userRepository = Substitute.For<IUserRepository>();
        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        var handler = new LockUserHandler(currentUserProvider, userRepository);

        Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command));
    }

    [Test]
    public void LockCurrentUser()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User
        {
            Id = currentUser.Id,
            Login = currentUser.Login,
            DisplayName = currentUser.Login,
            Email = "test@localhost.local",
        };
        var command = new LockUser(currentUser.Login);

        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserByLogin(command.Login, Arg.Any<CancellationToken>())
            .Returns(user);
        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);

        var handler = new LockUserHandler(currentUserProvider, userRepository);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(command));
    }

    [Test]
    public async Task LockUser()
    {
        var currentUser = new CurrentUser(Guid.NewGuid(), "user", [], []);
        var user = new User
        {
            Login = "test",
            DisplayName = "test",
            Email = "test@localhost.local",
        };
        var command = new LockUser(user.Login);

        var userRepository = Substitute.For<IUserRepository>();
        userRepository
            .GetUserByLogin(command.Login, Arg.Any<CancellationToken>())
            .Returns(user);
        var currentUserProvider = Substitute.For<ICurrentUserProvider>();
        currentUserProvider
            .GetCurrentUser()
            .Returns(currentUser);

        var handler = new LockUserHandler(currentUserProvider, userRepository);
        await handler.Handle(command);

        Assert.That(user.IsLocked, Is.True);
    }
}