import AppLayout from '@/layout/AppLayout.vue';

export default {
    path: '/',
    component: AppLayout,
    children: [
        {
            path: '/',
            alias: '/manage/project',
            name: 'manageprojectlist',
            component: () => import('@/views/pages/manage/ProjectList.vue')
        }
    ]
};
