import AppLayout from '@/layout/AppLayout.vue';

export default {
    path: '/',
    component: AppLayout,
    children: [
        {
            path: '/',
            alias: '/dashboard',
            name: 'dashboard',
            component: () => import('@/views/Dashboard.vue')
        },
        {
            path: '/manage/project',
            name: 'manageprojectlist',
            component: () => import('@/views/pages/manage/ProjectList.vue')
        }
    ]
};
