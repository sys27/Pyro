// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using System.Text.Json.Serialization;
using Pyro.Domain.GitRepositories;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;

namespace Pyro;

[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    AllowTrailingCommas = true,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(CreateGitRepositoryRequest))]
[JsonSerializable(typeof(CreateUserRequest))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RefreshTokenRequest))]
[JsonSerializable(typeof(UpdateUserProfileRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
[JsonSerializable(typeof(CommitInfoResponse))]
[JsonSerializable(typeof(CommitUserResponse))]
[JsonSerializable(typeof(DirectoryViewResponse))]
[JsonSerializable(typeof(DirectoryViewItemResponse))]
[JsonSerializable(typeof(GitRepositoryResponse))]
[JsonSerializable(typeof(PermissionResponse))]
[JsonSerializable(typeof(RoleResponse))]
[JsonSerializable(typeof(TokenPairResponse))]
[JsonSerializable(typeof(TokenResponse))]
[JsonSerializable(typeof(UserProfileResponse))]
[JsonSerializable(typeof(UserResponse))]
[JsonSerializable(typeof(GitRepositoryCreated))]
internal partial class PyroJsonContext : JsonSerializerContext
{
}