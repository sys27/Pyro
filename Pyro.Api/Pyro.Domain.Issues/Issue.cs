// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.Shared.Models;

namespace Pyro.Domain.Issues;

public class Issue : Entity
{
    private readonly List<IssueComment> comments = [];
    private readonly List<Label> labels = [];

    public int IssueNumber { get; init; }

    public required string Title { get; set; }

    public required GitRepository Repository { get; init; }

    public required User Author { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public User? Assignee { get; private set; }

    public IReadOnlyList<IssueComment> Comments
        => comments;

    public IReadOnlyList<Label> Labels
        => labels;

    public IssueComment? GetComment(Guid commentId)
        => comments.FirstOrDefault(x => x.Id == commentId);

    public IssueComment AddComment(string content, User author, DateTimeOffset createdAt)
    {
        var comment = new IssueComment
        {
            Issue = this,
            Content = content,
            Author = author,
            CreatedAt = createdAt,
        };

        comments.Add(comment);

        return comment;
    }

    public void DeleteComment(IssueComment comment)
        => comments.Remove(comment);

    public void AssignTo(User? assignee)
        => Assignee = assignee;

    public void AddLabel(Label label)
    {
        if (labels.Any(x => x.Name == label.Name))
            return;

        labels.Add(label);
    }

    public void ClearLabels()
        => labels.Clear();
}