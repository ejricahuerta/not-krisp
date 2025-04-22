export interface Repository {
    id: string;
    name: string;
    full_name: string;
    description: string | null;
    html_url: string;
    stargazers_count: number;
    forks_count: number;
    owner: {
        login: string;
        avatar_url: string;
    };
}

export interface Project {
    id: string;
    name: string;
    body: string | null;
    html_url: string;
    number: number;
    state: string;
    created_at: string;
    updated_at: string;
}

export interface Issue {
    id: string;
    number: number;
    title: string;
    body: string | null;
    html_url: string;
    state: string;
    created_at: string;
    updated_at: string;
    labels: {
        id: string;
        name: string;
        color: string;
    }[];
    assignees: {
        login: string;
        avatar_url: string;
    }[];
    pull_request?: {
        url: string;
        html_url: string;
        diff_url: string;
        patch_url: string;
    };
}

export interface User {
    login: string;
    name: string;
    email: string;
    avatar_url: string;
}

export interface GitHubIssue {
    id: string;
    number: number;
    title: string;
    body: string;
    html_url: string;
    state: string;
    created_at: string;
    updated_at: string;
    labels: {
        id: string;
        name: string;
        color: string;
    }[];
    assignees: {
        login: string;
        avatar_url: string;
    }[];
} 