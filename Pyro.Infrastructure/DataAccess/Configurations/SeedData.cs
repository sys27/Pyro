// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Identity.Models;
using static Pyro.Domain.Identity.Models.Permission;
using static Pyro.Domain.Identity.Models.Role;

namespace Pyro.Infrastructure.DataAccess.Configurations;

public static class SeedData
{
    public static readonly Permission[] Permissions =
    [
        new Permission { Id = Guid.Parse("F65AD9FD-A259-4598-803A-F85607C7566B"), Name = RepositoryView },
        new Permission { Id = Guid.Parse("EDF38B44-B150-46DF-BC79-ADAA3C01659F"), Name = RepositoryEdit },
        new Permission { Id = Guid.Parse("A740C470-34EA-46C4-8CA0-DC692E1FB423"), Name = RepositoryManage },

        new Permission { Id = Guid.Parse("95FED72D-90B3-4104-891E-A7DAE7EA4405"), Name = UserView },
        new Permission { Id = Guid.Parse("2C182139-085D-4851-AA3B-CA218EE77E70"), Name = UserEdit },
        new Permission { Id = Guid.Parse("E6A86676-0F74-4D00-A9B7-FC84A065D673"), Name = UserManage }
    ];

    public static readonly Role[] Roles =
    [
        new Role { Id = Guid.Parse("9AA993EB-E3DB-4FCE-BA9F-B0BB23395B9D"), Name = "Admin" },
        new Role { Id = Guid.Parse("36B9E20E-9B6B-461B-B129-D6A49FE4F4F8"), Name = "User" }
    ];

    public static readonly User[] Users =
    [
        new User
        {
            Id = Guid.Parse("F9BA057A-35B0-4D10-8326-702D8F7EC966"),
            Email = "pyro@localhost.local",
            Password = [239, 163, 54, 78, 41, 129, 181, 60, 27, 181, 100, 116, 243, 128, 253, 209, 87, 147, 27, 73, 138, 190, 50, 65, 18, 253, 153, 127, 194, 97, 240, 29, 179, 58, 68, 117, 170, 97, 172, 236, 70, 27, 167, 168, 87, 3, 66, 53, 11, 34, 206, 209, 211, 150, 81, 227, 19, 161, 249, 24, 45, 138, 206, 197],
            Salt = [109, 28, 230, 18, 208, 250, 67, 218, 171, 6, 152, 200, 162, 109, 186, 132],
        }
    ];

    public static readonly object[] RolePermissions =
    [
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryView).Id },
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryEdit).Id },
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(RepositoryManage).Id },
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserView).Id },
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserEdit).Id },
        new { RoleId = SeedData.GetRole(Admin).Id, PermissionId = SeedData.GetPermission(UserManage).Id },
        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(RepositoryView).Id },
        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(RepositoryEdit).Id },
        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(UserView).Id },
        new { RoleId = SeedData.GetRole("User").Id, PermissionId = SeedData.GetPermission(UserEdit).Id }
    ];

    public static readonly object[] UserRoles =
    [
        new { UserId = GetUser("pyro@localhost.local").Id, RoleId = GetRole(Admin).Id }
    ];

    public static Permission GetPermission(string name)
        => Permissions.Single(x => x.Name == name);

    public static Role GetRole(string name)
        => Roles.Single(x => x.Name == name);

    public static User GetUser(string email)
        => Users.Single(x => x.Email == email);
}