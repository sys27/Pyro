// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;

namespace Pyro.Infrastructure.DataAccess.Configurations;

public static class SeedData
{
    public static readonly Permission[] Permissions =
    {
        new Permission { Id = Guid.Parse("F65AD9FD-A259-4598-803A-F85607C7566B"), Name = Permission.RepositoryView },
        new Permission { Id = Guid.Parse("EDF38B44-B150-46DF-BC79-ADAA3C01659F"), Name = Permission.RepositoryEdit },
        new Permission { Id = Guid.Parse("A740C470-34EA-46C4-8CA0-DC692E1FB423"), Name = Permission.RepositoryManage },

        new Permission { Id = Guid.Parse("95FED72D-90B3-4104-891E-A7DAE7EA4405"), Name = Permission.UserView },
        new Permission { Id = Guid.Parse("2C182139-085D-4851-AA3B-CA218EE77E70"), Name = Permission.UserEdit },
        new Permission { Id = Guid.Parse("E6A86676-0F74-4D00-A9B7-FC84A065D673"), Name = Permission.UserManage },
    };

    public static readonly Role[] Roles =
    {
        new Role { Id = Guid.Parse("9AA993EB-E3DB-4FCE-BA9F-B0BB23395B9D"), Name = "Admin" },
        new Role { Id = Guid.Parse("36B9E20E-9B6B-461B-B129-D6A49FE4F4F8"), Name = "User" },
    };

    public static Permission GetPermission(string name)
        => Permissions.Single(x => x.Name == name);

    public static Role GetRole(string name)
        => Roles.Single(x => x.Name == name);
}