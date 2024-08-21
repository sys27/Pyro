// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

namespace Pyro.Domain.Issues.UnitTests.Models;

public class IssueStatusTests
{
    [Test]
    public void AddTransitionTest()
    {
        var repository = new GitRepository
        {
            Name = "Repository",
        };
        var from = new IssueStatus
        {
            Name = "From",
            Color = 0,
            Repository = repository,
        };
        var to = new IssueStatus
        {
            Name = "To",
            Color = 0,
            Repository = repository,
        };

        var transition = from.AddTransition(to);

        Assert.Multiple(() =>
        {
            Assert.That(transition.From, Is.EqualTo(from));
            Assert.That(transition.To, Is.EqualTo(to));

            Assert.That(from.FromTransitions, Has.Member(transition));
            Assert.That(to.ToTransitions, Has.Member(transition));
        });
    }

    [Test]
    public void RemoveTransitionTest()
    {
        var repository = new GitRepository
        {
            Name = "Repository",
        };
        var from = new IssueStatus
        {
            Name = "From",
            Color = 0,
            Repository = repository,
        };
        var to = new IssueStatus
        {
            Name = "To",
            Color = 0,
            Repository = repository,
        };

        var transition = from.AddTransition(to);
        from.RemoveTransition(transition);

        Assert.Multiple(() =>
        {
            Assert.That(from.FromTransitions, Is.Empty);
            Assert.That(to.ToTransitions, Is.Empty);
        });
    }

    private static IEnumerable<TestCaseData> GetDataForCanTransitionToTest()
    {
        var repository = new GitRepository
        {
            Name = "Repository",
        };
        var from = new IssueStatus
        {
            Name = "From",
            Color = 0,
            Repository = repository,
        };
        var to = new IssueStatus
        {
            Name = "To",
            Color = 0,
            Repository = repository,
        };
        from.AddTransition(to);

        yield return new TestCaseData(from, to)
            .Returns(true)
            .SetName("Can transition to status if transition exists");

        yield return new TestCaseData(from, from)
            .Returns(true)
            .SetName("Can transition to status if status is the same");

        yield return new TestCaseData(to, from)
            .Returns(false)
            .SetName("Cannot transition to status if transition does not exist");
    }

    [Test]
    [TestCaseSource(nameof(GetDataForCanTransitionToTest))]
    public bool CanTransitionToTest(IssueStatus from, IssueStatus to)
        => from.CanTransitionTo(to);
}