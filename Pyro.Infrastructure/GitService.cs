// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Identity.Models;
using Pyro.Domain.UserProfiles;

namespace Pyro.Infrastructure;

internal class GitService : IGitService
{
    private const string ReadmeFile = "README.md";

    private readonly Options options;
    private readonly ILogger<GitService> logger;
    private readonly TimeProvider timeProvider;
    private readonly IUserProfileRepository profileRepository;

    public GitService(
        IOptions<Options> options,
        ILogger<GitService> logger,
        TimeProvider timeProvider,
        IUserProfileRepository profileRepository)
    {
        this.options = options.Value;
        this.logger = logger;
        this.timeProvider = timeProvider;
        this.profileRepository = profileRepository;
    }

    private string GetGitPath(GitRepository repository)
        => Path.Combine(options.BasePath, $"{repository.Name}.git");

    public async Task InitializeRepository(
        GitRepository repository,
        CancellationToken cancellationToken = default)
    {
        var pyroUser = await profileRepository.GetUserProfile(User.PyroUser, cancellationToken) ??
                       throw new InvalidOperationException("Pyro user not found");

        var gitPath = GetGitPath(repository);
        gitPath = Repository.Init(gitPath, true);

        // TODO:
        // var postUpdateHookPath = Path.Combine(gitPath, "hooks", "post-update");
        // const string postUpdateContent =
        //     """
        //     #!/bin/sh
        //     exec git update-server-info
        //     """;
        // await File.WriteAllTextAsync(postUpdateHookPath, postUpdateContent, cancellationToken);
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

    public DirectoryView GetDirectoryView(GitRepository repository)
    {
        var gitPath = GetGitPath(repository);
        using var gitRepo = new Repository(gitPath);
        var branch = gitRepo.Head;
        var lastCommit = branch.Tip;
        var lastCommitAuthor = lastCommit.Author;

        var commit = new CommitInfo(
            lastCommit.Sha,
            new CommitUser(lastCommitAuthor.Name, lastCommitAuthor.Email),
            lastCommit.Message,
            lastCommitAuthor.When);

        var items = lastCommit.Tree
            .Select(x =>
            {
                var associatedCommit = GetLastCommitWhereBlobChanged(branch, x);

                return new DirectoryViewItem(
                    x.Name,
                    x.Mode == Mode.Directory,
                    associatedCommit.Message,
                    associatedCommit.Author.When);
            })
            .OrderBy(x => !x.IsDirectory)
            .ThenBy(x => x.Name)
            .ToArray();

        return new DirectoryView(
            commit,
            items,
            branch.Commits.Count());
    }

    private Commit GetLastCommitWhereBlobChanged(Branch branch, TreeEntry treeEntry)
    {
        var lastCommit = branch.Tip;

        foreach (var commit in branch.Commits.Skip(1))
        {
            var existingTreeEntry = commit.Tree.FirstOrDefault(x => x.Name == treeEntry.Name);
            if (existingTreeEntry is null || existingTreeEntry.Target.Id != treeEntry.Target.Id)
                return lastCommit;

            lastCommit = commit;
        }

        return lastCommit;
    }

    public class Options
    {
        public const string Section = "Git";

        public required string BasePath { get; init; }
    }
}