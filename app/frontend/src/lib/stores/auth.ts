import { writable } from 'svelte/store';
import { browser } from '$app/environment';

export type UserInfo = {
    username: string;
    email: string | null;
    name: string | null;
    avatarUrl: string | null;
};

export type AuthState = {
    accessToken: string | null;
    isAuthenticated: boolean;
    userInfo: UserInfo | null;
};

function createAuthStore() {
    const { subscribe, set } = writable<AuthState>({
        accessToken: null,
        isAuthenticated: false,
        userInfo: null,
    });

    // Initialize from localStorage if available
    if (browser) {
        const storedToken = localStorage.getItem('accessToken');
        const storedUserInfo = localStorage.getItem('userInfo');
        if (storedToken) {
            set({
                accessToken: storedToken,
                isAuthenticated: true,
                userInfo: storedUserInfo ? JSON.parse(storedUserInfo) : null,
            });
        }
    }

    return {
        subscribe,
        setToken: (token: string | null, userInfo: UserInfo | null = null) => {
            if (browser) {
                localStorage.setItem('accessToken', token || '');
                if (userInfo) {
                    localStorage.setItem('userInfo', JSON.stringify(userInfo));
                }
            }
            set({
                accessToken: token,
                isAuthenticated: true,
                userInfo,
            });
        },
        clearToken: () => {
            if (browser) {
                localStorage.removeItem('accessToken');
                localStorage.removeItem('userInfo');
            }
            set({
                accessToken: null,
                isAuthenticated: false,
                userInfo: null,
            });
        },
        logout: () => {
            if (browser) {
                localStorage.removeItem('accessToken');
                localStorage.removeItem('userInfo');
            }
            set({
                accessToken: null,
                isAuthenticated: false,
                userInfo: null,
            });
        },
    };
}

export const auth = createAuthStore(); 