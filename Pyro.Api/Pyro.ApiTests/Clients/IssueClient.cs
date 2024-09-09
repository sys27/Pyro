// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses.Issues;

namespace Pyro.ApiTests.Clients;

internal class IssueClient : BaseClient
{
    public IssueClient()
    {
    }

    public IssueClient(Uri baseAddress)
        : base(baseAddress)
    {
    }

    public async Task<IReadOnlyList<IssueResponse>?> GetIssues(string repositoryName)
        => await Get<IReadOnlyList<IssueResponse>>($"/api/repositories/{repositoryName}/issues?size=50");

    public async Task<IssueResponse?> GetIssue(string repositoryName, int number)
        => await Get<IssueResponse>($"/api/repositories/{repositoryName}/issues/{number}");

    public async Task<IssueResponse?> CreateIssue(string repositoryName, CreateIssueRequest request)
        => await Post<IssueResponse>($"/api/repositories/{repositoryName}/issues", request);

    public async Task<IssueResponse?> UpdateIssue(string repositoryName, int number, UpdateIssueRequest request)
        => await Put<IssueResponse>($"/api/repositories/{repositoryName}/issues/{number}", request);

    public async Task DeleteIssue(string repositoryName, int number)
        => await Delete($"/api/repositories/{repositoryName}/issues/{number}");

    public async Task LockIssue(string repositoryName, int number)
        => await Post($"/api/repositories/{repositoryName}/issues/{number}/lock");

    public async Task UnlockIssue(string repositoryName, int number)
        => await Post($"/api/repositories/{repositoryName}/issues/{number}/unlock");

    public async Task<IReadOnlyList<IssueCommentResponse>?> GetComments(string repositoryName, int number)
        => await Get<IReadOnlyList<IssueCommentResponse>>($"/api/repositories/{repositoryName}/issues/{number}/comments");

    public async Task<IssueCommentResponse?> CreateComment(string repositoryName, int number, CreateIssueCommentRequest request)
        => await Post<IssueCommentResponse>($"/api/repositories/{repositoryName}/issues/{number}/comments", request);

    public async Task<IssueCommentResponse?> UpdateComment(string repositoryName, int number, Guid commentId, UpdateIssueCommentRequest request)
        => await Put<IssueCommentResponse>($"/api/repositories/{repositoryName}/issues/{number}/comments/{commentId}", request);

    public async Task DeleteComment(string repositoryName, int number, Guid commentId)
        => await Delete($"/api/repositories/{repositoryName}/issues/{number}/comments/{commentId}");

    public async Task<IReadOnlyList<IssueStatusResponse>?> GetIssueStatuses(string repositoryName)
        => await Get<IReadOnlyList<IssueStatusResponse>>($"/api/repositories/{repositoryName}/issues/statuses");

    public async Task<IssueStatusResponse?> GetIssueStatus(string repositoryName, Guid id)
        => await Get<IssueStatusResponse>($"/api/repositories/{repositoryName}/issues/statuses/{id}");

    public async Task<IssueStatusResponse?> CreateIssueStatus(
        string repositoryName,
        CreateIssueStatusRequest request)
        => await Post<IssueStatusResponse>($"/api/repositories/{repositoryName}/issues/statuses", request);

    public async Task<IssueStatusResponse?> UpdateIssueStatus(
        string repositoryName,
        Guid id,
        UpdateIssueStatusRequest request)
        => await Put<IssueStatusResponse>($"/api/repositories/{repositoryName}/issues/statuses/{id}", request);

    public async Task EnableIssueStatus(string repositoryName, Guid id)
        => await Post($"/api/repositories/{repositoryName}/issues/statuses/{id}/enable");

    public async Task DisableIssueStatus(string repositoryName, Guid id)
        => await Post($"/api/repositories/{repositoryName}/issues/statuses/{id}/disable");

    public async Task<IReadOnlyList<IssueStatusTransitionResponse>?> GetIssueStatusTransitions(string repositoryName)
        => await Get<IReadOnlyList<IssueStatusTransitionResponse>>($"/api/repositories/{repositoryName}/issues/statuses/transitions");

    public async Task<IssueStatusTransitionResponse?> CreateIssueStatusTransition(
        string repositoryName,
        CreateIssueStatusTransitionRequest request)
        => await Post<IssueStatusTransitionResponse>($"/api/repositories/{repositoryName}/issues/statuses/transitions", request);

    public async Task DeleteIssueStatusTransition(string repositoryName, Guid id)
        => await Delete($"/api/repositories/{repositoryName}/issues/statuses/transitions/{id}");

    public async Task<IReadOnlyList<IssueChangeLogResponse>?> GetIssueChangeLogs(
        string repositoryName,
        int issueNumber)
        => await Get<IReadOnlyList<IssueChangeLogResponse>>($"/api/repositories/{repositoryName}/issues/{issueNumber}/change-logs");

    public async Task<IReadOnlyList<UserResponse>?> GetUsers()
        => await Get<IReadOnlyList<UserResponse>>("/api/repositories/issues/users");
}