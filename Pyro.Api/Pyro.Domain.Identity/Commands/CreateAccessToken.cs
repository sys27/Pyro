// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;
using MediatR;
using Pyro.Domain.Core;
using Pyro.Domain.Core.Exceptions;
using Pyro.Domain.Identity.Models;

namespace Pyro.Domain.Identity.Commands;

public record CreateAccessToken(string Name, DateTimeOffset ExpiresAt) : IRequest<CreateAccessTokenResult>;

public record CreateAccessTokenResult(string Name, DateTimeOffset ExpiresAt, string Token);

public class CreateAccessTokenValidator : AbstractValidator<CreateAccessToken>
{
    public CreateAccessTokenValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ExpiresAt)
            .GreaterThan(timeProvider.GetUtcNow());
    }
}

public class CreateAccessTokenHandler : IRequestHandler<CreateAccessToken, CreateAccessTokenResult>
{
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository userRepository;
    private readonly IPasswordService passwordService;

    public CreateAccessTokenHandler(
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        this.currentUserProvider = currentUserProvider;
        this.userRepository = userRepository;
        this.passwordService = passwordService;
    }

    public async Task<CreateAccessTokenResult> Handle(CreateAccessToken request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await userRepository.GetUserById(currentUser.Id, cancellationToken);
        if (user is null)
            throw new DomainException("User not found");

        var token = passwordService.GeneratePassword();
        var (tokenHash, salt) = passwordService.GeneratePasswordHash(token);
        var accessToken = new AccessToken
        {
            Name = request.Name,
            ExpiresAt = request.ExpiresAt,
            Token = tokenHash,
            Salt = salt,
        };
        user.AddAccessToken(accessToken);

        return new CreateAccessTokenResult(accessToken.Name, accessToken.ExpiresAt, token);
    }
}