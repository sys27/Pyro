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
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.Shared;
using Pyro.Endpoints;
using Pyro.Extensions;
using Pyro.Infrastructure;
using Pyro.Services;

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddSignalR(options => options.EnableDetailedErrors = builder.Environment.IsDevelopment());
builder.Services.AddSingleton<IUserIdProvider, LoginUserIdProvider>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

builder.Services.AddIdentityDomain();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddValidatorsFromAssemblyContaining<GitRepository>()
    .AddValidatorsFromAssemblyContaining<User>();
builder.Services.AddMediatR(c => c
    .RegisterServicesFromAssemblyContaining<GitRepository>()
    .RegisterServicesFromAssemblyContaining<User>()
    .RegisterServicesFromAssemblyContaining<Pyro.Program>()
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

app.MapGroup("/api")
    .RequireAuthorization()
    .MapIdentityEndpoints()
    .MapProfileEndpoints()
    .MapGitRepositoryEndpoints();

app.MapHub<PyroHub>(
        "/signalr",
        options => options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents)
    .RequireAuthorization();

app.MapGitBackendEndpoints();

app.ApplyMigrations();
app.Run();

namespace Pyro
{
    public partial class Program
    {
    }
}