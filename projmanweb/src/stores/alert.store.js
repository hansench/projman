import { defineStore } from 'pinia';

export const useAlertStore = defineStore({
    id: 'alert',
    state: () => ({
        alert: null
    }),
    actions: {
        success(title, message) {
            this.alert = { title, message, severity: 'success' };
        },
        warning(title, message) {
            this.alert = { title, message, severity: 'warn' };
        },
        error(title, message) {
            this.alert = { title, message, severity: 'error' };
        },
        clear() {
            this.alert = null;
        }
    }
});
