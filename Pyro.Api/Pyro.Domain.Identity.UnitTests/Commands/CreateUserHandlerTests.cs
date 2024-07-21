// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using NSubstitute;
using Pyro.Domain.Identity.Commands;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.Identity.UnitTests.Commands;

public class CreateUserHandlerTests
{
    [Test]
    public async Task CreateValidUser()
    {
        var command = new CreateUser("test", "password", ["admin"]);

        var repository = Substitute.For<IUserRepository>();
        repository
            .GetRolesAsync()
            .Returns([new Role { Name = "admin" }]);

        var passwordService = Substitute.For<IPasswordService>();
        passwordService
            .GeneratePasswordHash(command.Password)
            .Returns((new byte[64], new byte[16]));

        var handler = new CreateUserHandler(repository, passwordService);

        var user = await handler.Handle(command);

        Assert.Multiple(() =>
        {
            Assert.That(user.Login, Is.EqualTo(command.Login));
            Assert.That(user.Roles, Has.Count.EqualTo(1));
            Assert.That(user.Roles[0].Name, Is.EqualTo("admin"));
        });
    }

    [Test]
    public void CreateUserWithInvalidRole()
    {
        var command = new CreateUser("test", "password", ["user"]);

        var repository = Substitute.For<IUserRepository>();
        repository
            .GetRolesAsync()
            .Returns([new Role { Name = "admin" }]);

        var passwordService = Substitute.For<IPasswordService>();
        passwordService
            .GeneratePasswordHash(command.Password)
            .Returns((new byte[64], new byte[16]));

        var handler = new CreateUserHandler(repository, passwordService);

        Assert.ThrowsAsync<DomainException>(() => handler.Handle(command));
    }
}