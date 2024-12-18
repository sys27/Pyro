// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using Microsoft.Extensions.Options;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity;
using Pyro.Domain.Shared.CurrentUserProvider;
using Pyro.Domain.Shared.Exceptions;
using Pyro.Infrastructure;

namespace Pyro.Services;

public class NativeGitBackend
{
    private readonly HttpContext httpContext;
    private readonly IOptions<GitOptions> gitOptions;
    private readonly IGitRepositoryRepository repository;
    private readonly ICurrentUserProvider currentUserProvider;
    private readonly IUserRepository userRepository;

    // TODO: move to infrastructure?
    // TODO: remove HttpContext dependency
    public NativeGitBackend(
        IHttpContextAccessor httpContextAccessor,
        IOptions<GitOptions> gitOptions,
        IGitRepositoryRepository repository,
        ICurrentUserProvider currentUserProvider,
        IUserRepository userRepository)
    {
        this.httpContext = httpContextAccessor.HttpContext ??
                           throw new ArgumentNullException(null, "HttpContext is not available");

        this.gitOptions = gitOptions;
        this.repository = repository;
        this.currentUserProvider = currentUserProvider;
        this.userRepository = userRepository;
    }

    public async Task GetHead(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = "/HEAD",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetInfoRefs(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = "/info/refs",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetInfoAlternates(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = "/info/alternates",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetInfoHttpAlternates(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = "/info/http-alternates",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetInfoPacks(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = "/info/packs",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetObjects(
        string repositoryName,
        string hash1,
        string hash2,
        CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = $"/objects/{hash1}/{hash2}",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetObjectsPack(
        string repositoryName,
        string hash,
        CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = $"/objects/pack/pack-{hash}.pack",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GetObjectsPackIdx(
        string repositoryName,
        string hash,
        CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "GET",
            PathInfo = $"/objects/pack/pack-{hash}.idx",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GitUploadPack(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "POST",
            PathInfo = "/git-upload-pack",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GitUploadArchive(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "POST",
            PathInfo = "/git-upload-archive",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    public async Task GitReceivePack(string repositoryName, CancellationToken cancellationToken = default)
    {
        var parameters = new GitRequestParameters
        {
            RepositoryName = repositoryName,
            GitProtocol = httpContext.Request.Headers["Git-Protocol"].ToString(),
            RequestMethod = "POST",
            PathInfo = "/git-receive-pack",
            QueryString = httpContext.Request.QueryString.ToUriComponent().TrimStart('?'),
            ContentType = httpContext.Request.ContentType ?? string.Empty,
            ContentLength = httpContext.Request.ContentLength ?? 0,
            ContentEncoding = httpContext.Request.Headers.ContentEncoding.ToString(),
            RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
        };

        await Handle(parameters, cancellationToken);
    }

    private async Task Handle(GitRequestParameters parameters, CancellationToken cancellationToken = default)
    {
        var gitRepository = await repository.GetGitRepository(parameters.RepositoryName, cancellationToken) ??
                            throw new NotFoundException("Repository not found");

        var currentUser = currentUserProvider.GetCurrentUser();
        var user = await userRepository.GetUserById(currentUser.Id, cancellationToken) ??
                   throw new NotFoundException($"User '{currentUser.Id}' not found");

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
                { "HTTP_GIT_PROTOCOL", parameters.GitProtocol },
                { "REQUEST_METHOD", parameters.RequestMethod },
                { "GIT_PROJECT_ROOT", gitPath },
                { "PATH_INFO", parameters.PathInfo },
                { "QUERY_STRING", parameters.QueryString },
                { "CONTENT_TYPE", parameters.ContentType },
                { "CONTENT_LENGTH", parameters.ContentLength.ToString(CultureInfo.InvariantCulture) },
                { "HTTP_CONTENT_ENCODING", parameters.ContentEncoding },
                { "REMOTE_USER", currentUser.Login },
                { "REMOTE_ADDR", parameters.RemoteAddress },
                { "GIT_COMMITTER_NAME", user.DisplayName },
                { "GIT_COMMITTER_EMAIL", user.Email },
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

    private readonly struct GitRequestParameters
    {
        public required string RepositoryName { get; init; }

        public required string GitProtocol { get; init; }

        public required string RequestMethod { get; init; }

        public required string PathInfo { get; init; }

        public required string QueryString { get; init; }

        public required string ContentType { get; init; }

        public required long ContentLength { get; init; }

        public required string ContentEncoding { get; init; }

        public required string RemoteAddress { get; init; }
    }
}