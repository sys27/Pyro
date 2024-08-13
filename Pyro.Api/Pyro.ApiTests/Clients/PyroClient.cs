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

    public async Task<IReadOnlyList<TagResponse>?> GetTags(string repositoryName)
        => await Get<IReadOnlyList<TagResponse>>($"/api/repositories/{repositoryName}/tags");

    public async Task<TagResponse?> GetTag(string repositoryName, Guid id)
        => await Get<TagResponse>($"/api/repositories/{repositoryName}/tags/{id}");

    public async Task<TagResponse?> CreateTag(string repositoryName, CreateTagRequest request)
        => await Post<TagResponse>($"/api/repositories/{repositoryName}/tags", request);

    public async Task<TagResponse?> UpdateTag(string repositoryName, Guid id, UpdateTagRequest request)
        => await Put<TagResponse>($"/api/repositories/{repositoryName}/tags/{id}", request);

    public async Task DeleteTag(string repositoryName, Guid id)
        => await Delete($"/api/repositories/{repositoryName}/tags/{id}");
}