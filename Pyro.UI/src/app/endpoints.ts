export class Endpoints {
    public static readonly Login: string = '/api/identity/login';
    public static readonly Logout: string = '/api/identity/logout';
    public static readonly Refresh: string = '/api/identity/refresh';

    public static readonly Users: string = '/api/users';
    public static readonly Roles: string = '/api/roles';
    public static readonly Permissions: string = '/api/permissions';

    public static readonly Repositories: string = '/api/repositories';
    public static readonly Profile: string = '/api/profile';
    public static readonly AccessTokens: string = `${this.Users}/access-tokens`;

    public static readonly IssueUsers: string = `/api/repositories/issues/users`;
    public static Issues(repositoryName: string): string {
        return `/api/repositories/${repositoryName}/issues`;
    }

    public static Labels(repositoryName: string): string {
        return `/api/repositories/${repositoryName}/labels`;
    }

    public static Statuses(repositoryName: string): string {
        return `/api/repositories/${repositoryName}/issues/statuses`;
    }

    public static StatusTransitions(repositoryName: string): string {
        return `/api/repositories/${repositoryName}/issues/statuses/transitions`;
    }

    public static IssueChangeLogs(repositoryName: string, issueNumber: number): string {
        return `/api/repositories/${repositoryName}/issues/${issueNumber}/change-logs`;
    }
}
