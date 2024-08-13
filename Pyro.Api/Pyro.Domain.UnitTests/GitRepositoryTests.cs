// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Domain.UnitTests;

public class GitRepositoryTests
{
    [Test]
    public void AddTag()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        var tag = repository.AddTag(name, 0);

        Assert.Multiple(() =>
        {
            Assert.That(repository.Tags, Has.Count.EqualTo(1));
            Assert.That(repository.Tags, Has.One.EqualTo(tag));
            Assert.That(tag.Name, Is.EqualTo(name));
            Assert.That(tag.Color, Is.EqualTo(0));
        });
    }

    [Test]
    public void AddTagSameTagTwice()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        repository.AddTag(name, 0);

        Assert.Throws<DomainException>(() => repository.AddTag(name, 0));
    }

    [Test]
    public void RemoveTag()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        const string name = "Test";
        var tag = repository.AddTag(name, 0);
        repository.RemoveTag(tag.Id);

        Assert.That(repository.Tags, Is.Empty);
    }

    [Test]
    public void RemoveTagMissingTag()
    {
        var repository = new GitRepository
        {
            Name = "Test",
            DefaultBranch = "main",
        };

        Assert.Throws<DomainException>(() => repository.RemoveTag(Guid.NewGuid()));
    }
}