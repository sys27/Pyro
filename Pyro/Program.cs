// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Pyro;
using Pyro.BackgroundServices;
using Pyro.Domain.Core;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Endpoints;
using Pyro.Extensions;
using Pyro.Infrastructure;
using Pyro.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JsonSerializerOptions>(options =>
    options.TypeInfoResolver = PyroJsonContext.Default);
builder.Services.AddProblemDetails(builder.Environment);

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddIdentityDomain();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<GitRepository>();
builder.Services.AddMediatR(c => c
    .RegisterServicesFromAssemblyContaining<GitRepository>()
    .RegisterServicesFromAssemblyContaining<User>()
    .RegisterServicesFromAssemblyContaining<Program>()
    .AddOpenBehavior(typeof(LoggingPipeline<,>))
    .AddOpenBehavior(typeof(ValidatorPipeline<,>)));

builder.Services.AddHostedService<OutboxMessageProcessing>();

builder.Services.AddAuth();

builder.Services.AddSpaStaticFiles(options =>
    options.RootPath = Path.Combine(Directory.GetCurrentDirectory(), "../Pyro.UI/dist/browser"));

builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(p => p.Expire(TimeSpan.FromHours(1)).Tag("permissions"));
    options.AddBasePolicy(p => p.Expire(TimeSpan.FromHours(1)).Tag("roles"));
    options.AddBasePolicy(p => p.Tag("all"));
});

builder.Services.AddTransient<IStartupFilter, MigrationStartupFilter>();

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