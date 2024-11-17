// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pyro.Domain.Git;
using Pyro.Domain.GitRepositories;
using Pyro.Domain.Shared.Exceptions;

namespace Pyro.Infrastructure;

internal class GitService : IGitService
{
    private const string ReadmeFile = "README.md";

    private readonly GitOptions options;
    private readonly ILogger<GitService> logger;
    private readonly TimeProvider timeProvider;

    public GitService(
        IOptions<GitOptions> options,
        ILogger<GitService> logger,
        TimeProvider timeProvider)
    {
        this.options = options.Value;
        this.logger = logger;
        this.timeProvider = timeProvider;
    }

    private string GetGitPath(GitRepository repository)
        => Path.Combine(options.BasePath, $"{repository.Name}.git");

    private static void MakeFileExecutable(string filePath)
    {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = "chmod",
            Arguments = $"+x \"{filePath}\"",
            UseShellExecute = false,
        };

        process.Start();
        process.WaitForExit();
    }

    public async Task InitializeRepository(
        GitRepository gitRepo,
        CancellationToken cancellationToken = default)
    {
        var gitPath = GetGitPath(gitRepo);
        gitPath = Repository.Init(gitPath, true);

        var postUpdateHookPath = Path.Combine(gitPath, "hooks", "post-update");

        // lang=sh
        const string postUpdateContent =
            """
            #!/bin/sh
            exec git update-server-info
            """;
        await File.WriteAllTextAsync(postUpdateHookPath, postUpdateContent, cancellationToken);
        MakeFileExecutable(postUpdateHookPath);

        var clonePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        clonePath = Repository.Clone(gitPath, clonePath);

        // TODO: use user identity
        const string pyroName = "Pyro";
        const string pyroEmail = "pyro@localhost.local";
        var identity = new Identity(pyroName, pyroEmail);
        var signature = new Signature(identity, timeProvider.GetUtcNow());
        using var repo = new Repository(clonePath, new RepositoryOptions { Identity = identity });

        var content = $"# {gitRepo.Name}";
        var readmePath = Path.Combine(repo.Info.WorkingDirectory, ReadmeFile);
        await File.WriteAllTextAsync(readmePath, content, cancellationToken);

        Commands.Stage(repo, ReadmeFile);
        repo.Commit("Initial commit", signature, signature);
        var branch = gitRepo.DefaultBranch == "master"
            ? repo.Branches[gitRepo.DefaultBranch]
            : repo.Branches.Rename(repo.Head, gitRepo.DefaultBranch);

        var origin = repo.Network.Remotes["origin"];
        repo.Network.Push(origin, branch.CanonicalName);

        Directory.Delete(clonePath, true);

        logger.LogInformation("Repository {Name} initialized", gitRepo.Name);
    }

    public IReadOnlyList<BranchItem> GetBranches(GitRepository gitRepo)
    {
        var gitPath = GetGitPath(gitRepo);
        using var repository = new Repository(gitPath);
        var branches = repository.Branches
            .Select(x => new BranchItem(
                x.FriendlyName,
                GetCommitInfo(x.Tip),
                x.FriendlyName == gitRepo.DefaultBranch))
            .OrderBy(x => x.Name != gitRepo.DefaultBranch)
            .ThenBy(x => x.Name)
            .ToArray();

        return branches;
    }

    private CommitInfo GetCommitInfo(Commit commit)
    {
        var author = commit.Author;

        return new CommitInfo(
            commit.Sha,
            new CommitUser(author.Name, author.Email),
            commit.MessageShort,
            author.When);
    }

    public TreeView GetTreeView(GitRepository gitRepo, string? branchOrPath = null)
    {
        var gitPath = GetGitPath(gitRepo);
        using var repository = new Repository(gitPath);
        var (lastCommit, path) = GetCommitAndPath(repository, gitRepo.DefaultBranch, branchOrPath);
        var commits = repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = lastCommit });
        var commitInfo = GetCommitInfo(lastCommit);

        var tree = GetTreeByPath(lastCommit, path);
        if (tree is null)
            throw new DomainException("Path not found");

        var items = tree
            .Select(x =>
            {
                var associatedCommit = GetLastCommitWhereBlobChanged(commits, path, x);

                return new TreeViewItem(
                    x.Name,
                    x.Mode == Mode.Directory,
                    associatedCommit.MessageShort,
                    associatedCommit.Author.When);
            })
            .OrderBy(x => !x.IsDirectory)
            .ThenBy(x => x.Name)
            .ToArray();

        return new TreeView(
            commitInfo,
            items,
            commits.Count());
    }

    private (Commit Commit, string? Path) GetCommitAndPath(
        Repository repository,
        string defaultBranch,
        string? branchOrPath)
    {
        if (string.IsNullOrWhiteSpace(branchOrPath))
            return (repository.Branches[defaultBranch].Tip, null);

        if (!branchOrPath.Contains('/'))
            return (repository.Branches[branchOrPath].Tip, null);

        var end = -1;

        while (end < branchOrPath.Length)
        {
            end = branchOrPath.IndexOf('/', end + 1);
            if (end == -1)
                end = branchOrPath.Length;

            var branchName = branchOrPath[..end];
            var branch = repository.Branches[branchName];
            if (branch is not null)
            {
                var pathStart = Math.Min(end + 1, branchOrPath.Length);
                return (branch.Tip, branchOrPath[pathStart..]);
            }
        }

        return (repository.Head.Tip, null);
    }

    private Tree? GetTreeByPath(Commit commit, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return commit.Tree;

        var treeEntry = commit[path];
        if (treeEntry is null)
            return null;

        if (treeEntry.Target is Tree tree)
            return tree;

        throw new DomainException("Tree entry is not a tree");
    }

    private Commit GetLastCommitWhereBlobChanged(ICommitLog commits, string? path, TreeEntry treeEntry)
    {
        var lastCommit = commits.First();
        foreach (var commit in commits.Skip(1))
        {
            var tree = GetTreeByPath(commit, path);
            if (tree is null)
                return lastCommit;

            var existingTreeEntry = tree.FirstOrDefault(x => x.Name == treeEntry.Name);
            if (existingTreeEntry is null || existingTreeEntry.Target.Id != treeEntry.Target.Id)
                return lastCommit;

            lastCommit = commit;
        }

        return lastCommit;
    }

    public GitFile GetFile(GitRepository gitRepo, string branchAndPath)
    {
        var gitPath = GetGitPath(gitRepo);
        using var repository = new Repository(gitPath);
        var (lastCommit, path) = GetCommitAndFile(repository, gitRepo.DefaultBranch, branchAndPath);
        var treeEntry = lastCommit[path];
        if (treeEntry is null)
            throw new DomainException("Path not found");

        if (treeEntry.Target is not Blob blob)
            throw new DomainException("Tree entry is not a blob");

        return new GitFile(
            treeEntry.Name,
            blob.GetContentStream(),
            blob.Size,
            blob.IsBinary);
    }

    private (Commit Commit, string? Path) GetCommitAndFile(
        Repository repository,
        string defaultBranch,
        string branchOrPath)
    {
        if (!branchOrPath.Contains('/'))
            return (repository.Branches[defaultBranch].Tip, branchOrPath);

        var end = -1;

        while (end < branchOrPath.Length)
        {
            end = branchOrPath.IndexOf('/', end + 1);
            if (end == -1)
                end = branchOrPath.Length;

            var branchName = branchOrPath[..end];
            var branch = repository.Branches[branchName];
            if (branch is not null)
            {
                var pathStart = Math.Min(end + 1, branchOrPath.Length);
                return (branch.Tip, branchOrPath[pathStart..]);
            }
        }

        return (repository.Head.Tip, null);
    }
}