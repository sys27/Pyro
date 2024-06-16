// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using JWT;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using JWT.Extensions.AspNetCore.Factories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Pyro;
using Pyro.BackgroundServices;
using Pyro.Domain.Core;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Endpoints;
using Pyro.Infrastructure;
using Pyro.Infrastructure.DataAccess;
using Pyro.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton(JsonSerializerOptions.Default);

// TODO: only needed for problem details (see https://github.com/khellang/Middleware/issues/182)
builder.Services.AddMvcCore();
builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (_, _) => builder.Environment.IsDevelopment();

    options.Map<ValidationException>(ex =>
    {
        var errors = ex.Errors
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(x => x.Key, x => x.ToArray());

        return new HttpValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.io/400",
        };
    });

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddIdentityDomain();

builder.Services.AddTransient<DomainEventInterceptor>();
builder.Services.AddDbContext<PyroDbContext>((provider, options) => options
    .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(provider.GetRequiredService<DomainEventInterceptor>()));
builder.Services.AddInfra();
builder.Services.AddValidatorsFromAssemblyContaining<GitRepository>();
builder.Services.AddMediatR(c => c
    .RegisterServicesFromAssemblyContaining<GitRepository>()
    .RegisterServicesFromAssemblyContaining<User>()
    .RegisterServicesFromAssemblyContaining<Program>()
    .AddOpenBehavior(typeof(LoggingPipeline<,>))
    .AddOpenBehavior(typeof(ValidatorPipeline<,>)));

builder.Services.AddHostedService<OutboxMessageProcessing>();

builder.Services.AddSingleton(_ => new ValidationParameters
{
    ValidateSignature = true,
    ValidateIssuedTime = true,
    ValidateExpirationTime = true,
    TimeMargin = 30,
});
builder.Services.AddJwtEncoder<HMACSHA512Algorithm>();
builder.Services.AddJwtDecoder<GenericAlgorithmFactory<HMACSHA512Algorithm>>();
builder.Services.AddSingleton<IIdentityFactory, PyroClaimsIdentityFactory>();
builder.Services.AddAuthentication().AddJwt(options =>
{
    options.VerifySignature = true;
    options.Keys = ["secret"]; // TODO:
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
builder.Services.AddSingleton<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddSpaStaticFiles(options =>
    options.RootPath = Path.Combine(Directory.GetCurrentDirectory(), "../Pyro.UI/dist/browser"));

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(p => p.Expire(TimeSpan.FromHours(1)).Tag("permissions"));
    options.AddBasePolicy(p => p.Expire(TimeSpan.FromHours(1)).Tag("roles"));
    options.AddBasePolicy(p => p.Tag("all"));
});

var app = builder.Build();

app.UseProblemDetails();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}
else
{
    app.UseWhen(
        context => !context.Request.Path.StartsWithSegments("/api"),
        then => then.UseSpa(spa =>
        {
            // TODO: release
            const int port = 4200;

            spa.Options.SourcePath = Path.Combine(Directory.GetCurrentDirectory(), "../Pyro.UI");
            spa.Options.DevServerPort = port;
            spa.Options.PackageManagerCommand = "npm";

            spa.UseAngularCliServer("asp");
            spa.UseProxyToSpaDevelopmentServer($"http://localhost:{port}");
        }));
}

app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.MapGroup("/api")
    .RequireAuthorization()
    .MapIdentityEndpoints()
    .MapProfileEndpoints()
    .MapGitRepositoryEndpoints();

app.Run();