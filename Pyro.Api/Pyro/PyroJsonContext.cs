// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Pyro.Contracts.Requests;
using Pyro.Contracts.Requests.Identity;
using Pyro.Contracts.Responses;
using Pyro.Contracts.Responses.Identity;
using Pyro.Domain.GitRepositories;

namespace Pyro;

[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    AllowTrailingCommas = true,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(CreateAccessTokenRequest))]
[JsonSerializable(typeof(CreateGitRepositoryRequest))]
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(UpdateUserProfileRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(AccessTokenResponse))]
[JsonSerializable(typeof(BranchItemResponse))]
[JsonSerializable(typeof(CommitInfoResponse))]
[JsonSerializable(typeof(CommitUserResponse))]
[JsonSerializable(typeof(GitRepositoryCreated))]
[JsonSerializable(typeof(GitRepositoryResponse))]
[JsonSerializable(typeof(PermissionResponse))]
[JsonSerializable(typeof(RoleResponse))]
[JsonSerializable(typeof(TokenPairResponse))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(TreeViewItemResponse))]
[JsonSerializable(typeof(TreeViewResponse))]
[JsonSerializable(typeof(UserProfileResponse))]
[JsonSerializable(typeof(UserResponse))]
internal partial class PyroJsonContext : JsonSerializerContext
{
}