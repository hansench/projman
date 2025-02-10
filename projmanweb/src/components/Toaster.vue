<script setup>
import { storeToRefs } from 'pinia';
import { watch } from 'vue';
import { useAlertStore } from '@/stores';
import { useToast } from 'primevue/usetoast';

const toast = useToast();
const alertStore = useAlertStore();
const { alert } = storeToRefs(alertStore);

const showToast = function (data) {
    // console.log(data);
    if (data) {
        let title = data.title ?? 'Info';
        let message = data.message ?? 'Info Message';
        let severity = data.severity ?? 'info';
        toast.add({ severity: severity, summary: title, detail: message, life: 3500 });
    }
};

watch(alert, () => {
    showToast(alert.value);
});
</script>

<template>
    <Toast />
</template>
