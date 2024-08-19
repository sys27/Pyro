// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.UnitTests;

public class GitRepositoryTests
{
    [Test]
    public void AddLabel()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        var label = repository.AddLabel(name, 0);

        Assert.Multiple(() =>
        {
            Assert.That(repository.Labels, Has.Count.EqualTo(1));
            Assert.That(repository.Labels, Has.One.EqualTo(label));
            Assert.That(label.Name, Is.EqualTo(name));
            Assert.That(label.Color, Is.EqualTo(0));
        });
    }

    [Test]
    public void AddLabelSameLabelTwice()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        repository.AddLabel(name, 0);

        Assert.Throws<DomainException>(() => repository.AddLabel(name, 0));
    }

    [Test]
    public void RemoveLabel()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        var label = repository.AddLabel(name, 0);
        repository.RemoveLabel(label.Id);

        Assert.That(repository.Labels, Is.Empty);
    }

    [Test]
    public void RemoveLabelMissingLabel()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        Assert.Throws<DomainException>(() => repository.RemoveLabel(Guid.NewGuid()));
    }
}