// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using Microsoft.Extensions.Options;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Domain.UserProfiles;
using Pyro.Infrastructure;

namespace Pyro.Services;

public class GitBackend
{
    private readonly HttpContext httpContext;
    private readonly IOptions<GitOptions> gitOptions;
    private readonly IGitRepositoryRepository repository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserProfileRepository userProfileRepository;

    // TODO: move to infrastructure?
    // TODO: remove HttpContent dependency
    public GitBackend(
        IHttpContextAccessor httpContextAccessor,
        IOptions<GitOptions> gitOptions,
        IGitRepositoryRepository repository,
        ICurrentUserProvider currentUserProvider,
        IUserProfileRepository userProfileRepository)
    {
        this.httpContext = httpContextAccessor.HttpContext ??
                           throw new ArgumentNullException(null, "HttpContext is not available");

        this.gitOptions = gitOptions;
        this.repository = repository;
        this.currentUserProvider = currentUserProvider;
        this.userProfileRepository = userProfileRepository;
    }

    public async Task Handle(string repositoryName, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(repositoryName, cancellationToken) ??
                            throw new NotFoundException("Repository not found");

        var currentUser = currentUserProvider.GetCurrentUser();
        var userProfile = await userProfileRepository.GetUserProfile(currentUser.Id, cancellationToken) ??
                          throw new NotFoundException("User profile not found");

        var basePath = gitOptions.Value.BasePath;
        var gitPath = Path.Combine(basePath, $"{gitRepository.Name}.git");

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "http-backend --stateless-rpc --advertise-refs",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = gitPath,
            EnvironmentVariables =
            {
                { "GIT_HTTP_EXPORT_ALL", "1" },
                { "HTTP_GIT_PROTOCOL", httpContext.Request.Headers["Git-Protocol"] },
                { "REQUEST_METHOD", httpContext.Request.Method },
                { "GIT_PROJECT_ROOT", gitPath },
                { "PATH_INFO", $"/{httpContext.Request.RouteValues["path"]}" },
                { "QUERY_STRING", httpContext.Request.QueryString.ToUriComponent().TrimStart('?') },
                { "CONTENT_TYPE", httpContext.Request.ContentType },
                { "CONTENT_LENGTH", httpContext.Request.ContentLength?.ToString(CultureInfo.InvariantCulture) },
                { "HTTP_CONTENT_ENCODING", httpContext.Request.Headers.ContentEncoding },
                { "REMOTE_USER", currentUser.Login },
                { "REMOTE_ADDR", httpContext.Connection.RemoteIpAddress?.ToString() },
                { "GIT_COMMITTER_NAME", currentUser.Login },
                { "GIT_COMMITTER_EMAIL", userProfile.User.Email },
            },
        };

        process.Start();

        var pipeWriter = PipeWriter.Create(process.StandardInput.BaseStream);
        await httpContext.Request.BodyReader.CopyToAsync(pipeWriter, cancellationToken);

        var pipeReader = PipeReader.Create(process.StandardOutput.BaseStream);
        await ReadResponse(pipeReader, cancellationToken);

        await pipeReader.CopyToAsync(httpContext.Response.BodyWriter, cancellationToken);
        await pipeReader.CompleteAsync();
    }

    private async Task ReadResponse(PipeReader pipeReader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await pipeReader.ReadAsync(cancellationToken);
            var buffer = result.Buffer;
            var (position, isFinished) = ReadHeaders(httpContext, buffer);
            pipeReader.AdvanceTo(position, buffer.End);

            if (result.IsCompleted || isFinished)
                break;
        }
    }

    private static (SequencePosition Position, bool IsFinished) ReadHeaders(
        HttpContext httpContext,
        in ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        while (!reader.End)
        {
            if (!reader.TryReadTo(out ReadOnlySpan<byte> line, (byte)'\n'))
                break;

            if (line.Length == 1)
                return (reader.Position, true);

            var colon = line.IndexOf((byte)':');
            if (colon == -1)
                break;

            var headerName = Encoding.UTF8.GetString(line[..colon]);
            var headerValue = Encoding.UTF8.GetString(line[(colon + 1)..]).Trim();
            httpContext.Response.Headers[headerName] = headerValue;
        }

        return (reader.Position, false);
    }
}