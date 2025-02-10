<script setup>
import { computed, defineProps } from 'vue';

const props = defineProps({
    stageId: {
        type: Number,
        required: true
    },
    stageName: {
        type: String,
        required: true
    }
});

const severity = computed(() => {
    switch (props.stageId) {
        case 1:
            return 'info';
        case 2:
            return 'primary';
        case 3:
            return 'warning';
        case 4:
            return 'success';
        default:
            return 'info';
    }
});

</script>

<template>
    <div>
        <Tag rounded :value="props.stageName" :severity="severity"></Tag>
    </div>
</template>
