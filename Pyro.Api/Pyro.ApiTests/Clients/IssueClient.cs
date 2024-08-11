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

    public async Task<IReadOnlyList<IssueCommentResponse>?> GetComments(string repositoryName, int number)
        => await Get<IReadOnlyList<IssueCommentResponse>>($"/api/repositories/{repositoryName}/issues/{number}/comments");

    public async Task<IssueCommentResponse?> CreateComment(string repositoryName, int number, CreateIssueCommentRequest request)
        => await Post<IssueCommentResponse>($"/api/repositories/{repositoryName}/issues/{number}/comments", request);

    public async Task<IssueCommentResponse?> UpdateComment(string repositoryName, int number, Guid commentId, UpdateIssueCommentRequest request)
        => await Put<IssueCommentResponse>($"/api/repositories/{repositoryName}/issues/{number}/comments/{commentId}", request);

    public async Task DeleteComment(string repositoryName, int number, Guid commentId)
        => await Delete($"/api/repositories/{repositoryName}/issues/{number}/comments/{commentId}");
}