import { writable } from 'svelte/store';
import type { Repository, Project, Issue } from '../types/github';

interface GitHubState {
    repositories: Repository[];
    projects: Project[];
    issues: Issue[];
    isLoading: boolean;
    error: string | null;
    user: {
        login: string;
        name: string;
        email: string;
        avatarUrl: string;
    } | null;
}

function createGitHubStore() {
    const { subscribe, set, update } = writable<GitHubState>({
        repositories: [],
        projects: [],
        issues: [],
        isLoading: false,
        error: null,
        user: null
    });

    return {
        subscribe,
        setRepositories: (repositories: Repository[]) => 
            update(state => ({ ...state, repositories, error: null })),
        setProjects: (projects: Project[]) => 
            update(state => ({ ...state, projects, error: null })),
        setIssues: (issues: Issue[]) => 
            update(state => ({ ...state, issues, error: null })),
        setLoading: (isLoading: boolean) => 
            update(state => ({ ...state, isLoading })),
        setError: (error: string | null) => 
            update(state => ({ ...state, error })),
        setUser: (user: GitHubState['user']) => 
            update(state => ({ ...state, user })),
        reset: () => set({
            repositories: [],
            projects: [],
            issues: [],
            isLoading: false,
            error: null,
            user: null
        })
    };
}

export const github = createGitHubStore(); 