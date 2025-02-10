import { defineStore } from 'pinia';

import Api from '@/service/api';
import { router } from '@/router';
import { useAlertStore } from '@/stores';

const baseUrl = `${import.meta.env.VITE_API_BASE_URL}/auth`;

export const useAuthStore = defineStore({
    id: 'auth',
    state: () => ({
        // initialize state from local storage to enable user to stay logged in
        user: JSON.parse(localStorage.getItem('user')),
        loading: false,
        returnUrl: null
    }),
    actions: {
        login(username, password) {
            this.loading = true;
            Api.post(`${baseUrl}/login`, { username, password })
                .then((response) => {
                    this.user = response.data.row;
                    localStorage.setItem('user', JSON.stringify(response.data.row));

                    // redirect to previous url or default to home page
                    router.push(this.returnUrl || '/');
                })
                .catch((error) => {
                    const alertStore = useAlertStore();
                    alertStore.error('Login Error', error.response.data.message);
                })
                .finally(() => {
                    this.loading = false;
                });
        },
        refreshToken() {
            var request = Api.post(`${baseUrl}/refresh-token`, { token: this.user.token, refreshToken: this.user.refreshToken })
                .then((response) => {
                    this.user = response.data.row;
                    localStorage.setItem('user', JSON.stringify(response.data.row));
                    return Promise.resolve();
                })
                .catch((err) => {
                    return Promise.reject(err);
                });

            return request;
        },
        logout() {
            this.user = null;
            localStorage.removeItem('user');
            router.push('/auth/login');
        }
    }
});
