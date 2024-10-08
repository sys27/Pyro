// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Pyro;
using Pyro.BackgroundServices;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Issues;
using Pyro.Domain.Shared;
using Pyro.Endpoints;
using Pyro.Extensions;
using Pyro.Infrastructure;
using Pyro.Infrastructure.Identity;
using Pyro.Infrastructure.Issues;
using Pyro.Infrastructure.Shared;
using Pyro.Services;
using User = Pyro.Domain.Identity.Models.User;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("ASPNETCORE_");

builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JsonSerializerOptions>(options =>
    options.TypeInfoResolver = PyroJsonContext.Default);
builder.Services.AddProblemDetails(builder.Environment);
builder.Services.AddHealthChecks();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());
builder.Services.AddSingleton<IUserIdProvider, LoginUserIdProvider>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

builder.Services.AddIdentityDomain();
builder.Services.AddSharedInfrastructure();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIssuesInfrastructure(builder.Configuration);

builder.Services
    .AddValidatorsFromAssemblyContaining<Pyro.Domain.GitRepositories.GitRepository>()
    .AddValidatorsFromAssemblyContaining<User>()
    .AddValidatorsFromAssemblyContaining<Issue>();
builder.Services.AddMediatR(c => c
    .RegisterServicesFromAssemblyContaining<Pyro.Domain.GitRepositories.GitRepository>()
    .RegisterServicesFromAssemblyContaining<Permission>()
    .RegisterServicesFromAssemblyContaining<Issue>()
    .RegisterServicesFromAssemblyContaining<Program>()
    .AddOpenBehavior(typeof(LoggingPipeline<,>))
    .AddOpenBehavior(typeof(ValidatorPipeline<,>)));

builder.Services.AddHostedService<OutboxMessageProcessing>();

builder.Services.AddAuth();

// TODO:
builder.Services.AddScoped<GitBackend>();

var app = builder.Build();

app.UseProblemDetails();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();

    app.UseFileServer(new FileServerOptions
    {
        EnableDefaultFiles = true,
        EnableDirectoryBrowsing = false,
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        DefaultFilesOptions =
        {
            DefaultFileNames = ["index.html"],
        },
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapGroup("/api")
    .RequireAuthorization()
    .MapIdentityEndpoints()
    .MapProfileEndpoints()
    .MapGitRepositoryEndpoints()
    .MapIssueEndpoints();

app.MapHub<PyroHub>(
        "/signalr",
        options => options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents)
    .RequireAuthorization();

app.MapGitBackendEndpoints();

app.ApplyMigrations();
app.Run();