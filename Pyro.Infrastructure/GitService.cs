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
        var branch = repo.Branches.Rename(repo.Head, repository.DefaultBranch);

        var origin = repo.Network.Remotes["origin"];
        repo.Network.Push(origin, branch.CanonicalName);

        Directory.Delete(clonePath, true);

        logger.LogInformation("Repository {Name} initialized", repository.Name);
    }

    public IReadOnlyList<BranchItem> GetBranches(GitRepository repository)
    {
        var gitPath = GetGitPath(repository);
        using var gitRepo = new Repository(gitPath);
        var branches = gitRepo.Branches
            .Select(x => new BranchItem(
                x.FriendlyName,
                GetCommitInfo(x.Tip),
                x.FriendlyName == repository.DefaultBranch))
            .OrderBy(x => x.Name != repository.DefaultBranch)
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

    public TreeView GetTreeView(GitRepository repository, string? branchOrPath = null)
    {
        var gitPath = GetGitPath(repository);
        using var gitRepo = new Repository(gitPath);
        var (lastCommit, path) = GetCommitAndPath(gitRepo, repository.DefaultBranch, branchOrPath);
        var commits = gitRepo.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = lastCommit });
        var commitInfo = GetCommitInfo(lastCommit);

        var tree = GetTreeByPath(lastCommit, path);
        if (tree is null)
            throw new InvalidOperationException("Path not found");

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
        Repository gitRepo,
        string defaultBranch,
        string? branchOrPath)
    {
        if (string.IsNullOrWhiteSpace(branchOrPath))
            return (gitRepo.Branches[defaultBranch].Tip, null);

        if (!branchOrPath.Contains('/'))
            return (gitRepo.Branches[branchOrPath].Tip, null);

        var end = -1;

        while (end < branchOrPath.Length)
        {
            end = branchOrPath.IndexOf('/', end + 1);
            if (end == -1)
                end = branchOrPath.Length;

            var branchName = branchOrPath[..end];
            var branch = gitRepo.Branches[branchName];
            if (branch is not null)
            {
                var pathStart = Math.Min(end + 1, branchOrPath.Length);
                return (branch.Tip, branchOrPath[pathStart..]);
            }
        }

        return (gitRepo.Head.Tip, null);
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

        throw new InvalidOperationException("Tree entry is not a tree");
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

    public class Options
    {
        public const string Section = "Git";

        public required string BasePath { get; init; }
    }
}