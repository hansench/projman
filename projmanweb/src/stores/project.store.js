import api from '@/service/api';
import { defineStore } from 'pinia';

export const useProjectStore = defineStore('ProjectStore', {
    id: 'project',
    actions: {
        fetchPagedList(params) {
            return api.post('/project/list', params);
        },
        fetch(id) {
            return api.get(`project/${id}`);
        },
        save(data) {
            if (data.id) {
                return api.put(`project`, data);
            }
            return api.post('project', data);
        }
    }
});
