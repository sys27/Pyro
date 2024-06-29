// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using Pyro.Domain.UserProfiles;
using Pyro.Dtos.Requests;
using Pyro.Dtos.Responses;
using Riok.Mapperly.Abstractions;

namespace Pyro.Dtos.Mapping;

[Mapper]
public static partial class DtoMapper
{
    [MapperIgnoreSource(nameof(UserProfile.Id))]
    [MapperIgnoreSource(nameof(UserProfile.Avatar))]
    public static partial UserProfileResponse ToResponse(this UserProfile request);

    public static partial UpdateProfile ToCommand(this UpdateUserProfileRequest request);
}