// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Pyro.Domain.Issues;

namespace Pyro.Infrastructure.Issues.DataAccess;

internal class IssueNumberValueGenerator : ValueGenerator<int>
{
    public override int Next(EntityEntry entry)
    {
        Debug.Assert(entry.Entity.GetType() == typeof(Issue), "This value generator is only for Issue entities");

        var repositoryId = entry.Property("RepositoryId").CurrentValue ??
                           throw new InvalidOperationException("RepositoryId is not set");

        var context = entry.Context;
        using var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
            connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT OR IGNORE INTO IssueNumberTracker (RepositoryId)
            VALUES (@repositoryId);

            UPDATE IssueNumberTracker
            SET Number = Number + 1
            WHERE RepositoryId = @repositoryId
            RETURNING Number;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@repositoryId";
        parameter.Value = repositoryId;
        command.Parameters.Add(parameter);

        var newNumber = command.ExecuteScalar() ??
                        throw new InvalidOperationException("Failed to generate issue number");

        return (int)(long)newNumber;
    }

    public override bool GeneratesTemporaryValues => false;
}