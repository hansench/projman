import Login from '@/views/pages/auth/Login.vue';

export default {
    path: '/auth',
    name: 'auth',
    children: [
        { path: '', name: 'login_index', redirect: 'login' },
        { path: 'login', name: 'login', component: Login }
    ]
};
