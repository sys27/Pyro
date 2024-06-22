// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pyro.Domain;
using Pyro.Domain.GitRepositories;

namespace Pyro.Infrastructure;

internal class GitService : IGitService
{
    private const string ReadmeFile = "README.md";

    private readonly Options options;
    private readonly ILogger<GitService> logger;
    private readonly TimeProvider timeProvider;

    public GitService(
        IOptions<Options> options,
        ILogger<GitService> logger,
        TimeProvider timeProvider)
    {
        this.options = options.Value;
        this.logger = logger;
        this.timeProvider = timeProvider;
    }

    private string GetGitPath(GitRepository repository)
        => Path.Combine(options.BasePath, $"{repository.Name}.git");

    public async Task InitializeRepository(
        GitRepository repository,
        CancellationToken cancellationToken = default)
    {
        // TODO:
        // var pyroUser = await userRepository.GetUserById(User.PyroUser, cancellationToken);
        // if (pyroUser is null)
        // {
        //     throw new InvalidOperationException("Pyro user not found");
        // }
        var pyroUser = new { Name = "pyro", Email = "pyro@localhost.local" };

        var gitPath = GetGitPath(repository);
        gitPath = Repository.Init(gitPath, true);

        var postUpdateHookPath = Path.Combine(gitPath, "hooks", "post-update");
        const string postUpdateContent =
            """
            #!/bin/sh
            exec git update-server-info
            """;
        await File.WriteAllTextAsync(postUpdateHookPath, postUpdateContent, cancellationToken);

        var clonePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        clonePath = Repository.Clone(gitPath, clonePath);

        var identity = new Identity(pyroUser.Name, pyroUser.Email);
        var signature = new Signature(identity, timeProvider.GetUtcNow());
        using var repo = new Repository(clonePath, new RepositoryOptions { Identity = identity });

        var content = $"# {repository.Name}";
        var readmePath = Path.Combine(repo.Info.WorkingDirectory, ReadmeFile);
        await File.WriteAllTextAsync(readmePath, content, cancellationToken);

        Commands.Stage(repo, ReadmeFile);
        repo.Commit("Initial commit", signature, signature);

        var origin = repo.Network.Remotes["origin"];
        repo.Network.Push(origin, "refs/heads/master");

        Directory.Delete(clonePath, true);

        logger.LogInformation("Repository {Name} initialized", repository.Name);
    }

    public class Options
    {
        public const string Section = "GitService";

        public required string BasePath { get; set; }
    }
}