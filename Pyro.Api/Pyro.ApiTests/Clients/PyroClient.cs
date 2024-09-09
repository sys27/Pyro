// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Issues;
using Pyro.Contracts.Responses;

namespace Pyro.ApiTests.Clients;

internal class PyroClient : BaseClient
{
    public PyroClient(Uri baseAddress)
        : base(baseAddress)
    {
    }

    public async Task<UserProfileResponse?> GetProfile()
        => await Get<UserProfileResponse>("/api/profile");

    public async Task UpdateProfile(UpdateUserProfileRequest request)
        => await Put("/api/profile", request);

    public async Task<IReadOnlyList<GitRepositoryResponse>?> GetGitRepositories()
        => await Get<IReadOnlyList<GitRepositoryResponse>>("/api/repositories?size=50");

    public async Task<GitRepositoryResponse?> GetGitRepository(string name)
        => await Get<GitRepositoryResponse>($"/api/repositories/{name}");

    public async Task<GitRepositoryResponse?> CreateGitRepository(CreateGitRepositoryRequest request)
        => await Post<GitRepositoryResponse>("/api/repositories", request);

    public async Task<IReadOnlyList<BranchItemResponse>?> GetBranches(string repositoryName)
        => await Get<IReadOnlyList<BranchItemResponse>>($"/api/repositories/{repositoryName}/branches");

    public async Task<TreeViewResponse?> GetTree(string repositoryName)
        => await Get<TreeViewResponse>($"/api/repositories/{repositoryName}/tree/master");

    public async Task<IReadOnlyList<LabelResponse>?> GetLabels(string repositoryName)
        => await Get<IReadOnlyList<LabelResponse>>($"/api/repositories/{repositoryName}/labels");

    public async Task<LabelResponse?> GetLabel(string repositoryName, Guid id)
        => await Get<LabelResponse>($"/api/repositories/{repositoryName}/labels/{id}");

    public async Task<LabelResponse?> CreateLabel(string repositoryName, CreateLabelRequest request)
        => await Post<LabelResponse>($"/api/repositories/{repositoryName}/labels", request);

    public async Task<LabelResponse?> UpdateLabel(string repositoryName, Guid id, UpdateLabelRequest request)
        => await Put<LabelResponse>($"/api/repositories/{repositoryName}/labels/{id}", request);

    public async Task EnableLabel(string repositoryName, Guid id)
        => await Post($"/api/repositories/{repositoryName}/labels/{id}/enable");

    public async Task DisableLabel(string repositoryName, Guid id)
        => await Post($"/api/repositories/{repositoryName}/labels/{id}/disable");
}