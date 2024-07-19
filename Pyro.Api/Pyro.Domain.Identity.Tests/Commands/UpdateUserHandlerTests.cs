// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.Tests.Commands;

public class UpdateUserHandlerTests
{
    [Test]
    public void UpdateUserWithInvalidRole()
    {
        var user = new User { Login = "test" };
        var updateUser = new UpdateUser(user, ["admin"]);

        var repository = Substitute.For<IUserRepository>();
        repository
            .GetRolesAsync()
            .Returns([]);
        var handler = new UpdateUserHandler(repository);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(updateUser));
    }

    [Test]
    public async Task UpdateUser()
    {
        var roles = new[]
        {
            new Role { Name = "admin" },
            new Role { Name = "user" },
        };
        var user = new User { Login = "test" };
        user.AddRole(roles[1]);

        var updateUser = new UpdateUser(user, [roles[0].Name]);

        var repository = Substitute.For<IUserRepository>();
        repository
            .GetRolesAsync()
            .Returns(roles);
        var handler = new UpdateUserHandler(repository);

        var updatedUser = await handler.Handle(updateUser);

        Assert.That(updatedUser.Roles, Has.Count.EqualTo(1));
        Assert.That(updatedUser.Roles, Has.One.EqualTo(roles[0]));
    }
}