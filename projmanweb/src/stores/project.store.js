import { defineStore } from 'pinia';
import api from '@/service/api';

export const useProjectStore = defineStore('ProjectStore', {
    id: 'project',
    actions: {
        fetchPagedList(params) {
            return api.post('/project/list', params);
        },
        fetch(ids) {
            return api.get(`project/${ids}`);
        }
    }
});
