import axios from 'axios';
// import { toRaw } from 'vue';
import { useAuthStore } from '@/stores';
import { router } from '@/router';

let isRefreshingToken = false;
let callbacks = [];

let api = axios.create({
    baseURL: import.meta.env.VITE_API_BASE_URL,
    headers: {
        Accept: 'application/json'
    }
});

api.interceptors.request.use((request) => {
    //add authorization header with jwt token to each request
    const authStore = useAuthStore();
    const user = authStore.user;
    // console.log(toRaw(authStore.user));
    if (user && user.token) {
        request.headers['Authorization'] = `Bearer ${user.token}`;
    }
    return request;
});

api.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        var errormessage = error.response && error.response.data.errors && error.response.data.errors.Error ? error.response.data.errors.Error : error.message;
        if (error.response && error.response.status === 422) {
            errormessage = '';
            error.response.data.errors.forEach((value) => {
                errormessage += value.toString() + ' ';
            });
        } else if (error.response && error.response.status === 401 && error.response.headers['token-expired']) {
            const authStore = useAuthStore();
            const user = authStore.user;
            if (user && user.refreshToken) {
                const originalRequest = error.config;
                if (!isRefreshingToken) {
                    isRefreshingToken = true;
                    authStore
                        .refreshToken()
                        .then(() => {
                            isRefreshingToken = false;
                            tokenRefreshed();
                        })
                        .catch(() => {
                            router.push('auth/login');
                        });
                }

                const retryOriginalRequest = new Promise((resolve) => {
                    addCallback(() => {
                        originalRequest.headers.Authorization = `Bearer ${user.token}`;
                        resolve(api(originalRequest));
                    });
                });
                return retryOriginalRequest;
            }
        }

        return Promise.reject(error);
    }
);

let tokenRefreshed = () => {
    callbacks = callbacks.filter((callback) => callback());
};

let addCallback = (callback) => {
    callbacks.push(callback);
};

export default api;
