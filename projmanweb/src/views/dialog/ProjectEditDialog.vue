<script setup>
import { useAlertStore } from "@/stores/alert.store";
import { useProjectStore } from "@/stores/project.store";
import { computed, defineProps, ref, toRaw, watch } from 'vue';
// import { useSelectListStore } from "@/stores/select-list.store"
import { useForm, useIsFormDirty, useIsFormValid } from 'vee-validate';
import * as Yup from 'yup';

const props = defineProps({
    projectData: {
        type: Object,
        required: false,
        default: () => ({
            id: 0,
            projectName: '',
            projectLocation: '',
            projectDetails: '',
            stageId: 1,
            categoryId: 1,
            categoryOthersDescr: '',
            startDate: new Date(),
        })
    },
    isDialogVisible: {
        type: Boolean,
        required: true
    },
    dialogTitle: {
        type: String,
        required: true,
        default: 'Add Project'
    }
});

const emit = defineEmits([
  'submit',
  'update:isDialogVisible',
])

const alertStore = useAlertStore();
const projectStore = useProjectStore();
// const selectListStore = useSelectListStore();

const isDirty = useIsFormDirty();
const isValid = useIsFormValid();

const isEnableSubmit = computed(() => {
  return isDirty && isValid
})

const isDisabled = computed(() => {
  return !isEnableSubmit
})

const loading = ref(false);
const projectData = ref(structuredClone(toRaw(props.projectData)));
const isEdit = ref(true);
const stageSelectList = ref([]);
const categorySelectList = ref([
    { id: 1, name: 'Category 1' },
    { id: 2, name: 'Category 2' },
    { id: 3, name: 'Category 3' },
    { id: 4, name: 'Others' },
]);

const schema = Yup.object().shape({
    projectName: Yup.string()
        .label('Project Name')
        .required((data) => `${data.label} is required`)
        .min(3, (data) => `${data.label} must be at least ${data.min} characters`)
        .max(200, (data) => `${data.label} must be at most ${data.max} characters`),
    projectLocation: Yup.string()
        .label('Project Location')
        .required((data) => `${data.label} is required`)
        .min(3, (data) => `${data.label} must be at least ${data.min} characters`)
        .max(500, (data) => `${data.label} must be at most ${data.max} characters`),
    projectDetails: Yup.string()
        .label('Project Details')
        .required((data) => `${data.label} is required`)
        .min(3, (data) => `${data.label} must be at least ${data.min} characters`)
        .max(2000, (data) => `${data.label} must be at most ${data.max} characters`),
    stageId: Yup.number()
        .label('Stage')
        .required((data) => `${data.label} is required`),
    categoryId: Yup.number()
        .label('Category')
        .required((data) => `${data.label} is required`),
    categoryOthersDescr: Yup.string()
        .label('Category Others Description')
        .when('categoryId', {
            is: 4,
            then: () => Yup.string()
                .required((data) => `${data.label} is required`)
                .min(3, (data) => `${data.label} must be at least ${data.min} characters`)
                .max(200, (data) => `${data.label} must be at most ${data.max} characters`),
            otherwise: () => Yup.string()
        }),
    startDate: Yup.date()
        .label('Start Date')
        .required((data) => `${data.label} is required`)
        .when('stageId', {
            is: 4,
            then: () => Yup.date(),
            otherwise: () => Yup.date()
                .min(new Date(), (data) => `${data.label} must be greater than today`)
        })
})

const { defineField, setFieldValue, handleSubmit, errors, isSubmitting, resetForm } = useForm({
  validationSchema: schema,
})

const [id] = defineField('id');
const [projectName] = defineField('projectName');
const [projectLocation] = defineField('projectLocation');
const [projectDetails] = defineField('projectDetails');
const [stageId] = defineField('stageId');
const [categoryId] = defineField('categoryId');
const [categoryOthersDescr] = defineField('categoryOthersDescr');
const [startDate] = defineField('startDate');

const setValues = () => {
  setFieldValue('id', projectData.value.id);
  setFieldValue('projectName', projectData.value.projectName);
  setFieldValue('projectLocation', projectData.value.projectLocation);
  setFieldValue('projectDetails', projectData.value.projectDetails);
  setFieldValue('stageId', projectData.value.stageId);
  setFieldValue('categoryId', projectData.value.categoryId);
  setFieldValue('categoryOthersDescr', projectData.value.categoryOthersDescr);
  setFieldValue('startDate', projectData.value.startDate);
}

const clearValues = () => {
    id.value = 0;
    projectName.value = '';
    projectLocation.value = '';
    projectDetails.value = '';
    stageId.value = 1;
    categoryId.value = 1;
    categoryOthersDescr.value = '';
    startDate.value = new Date();
    resetForm();
}

watch(props, () => {
  projectData.value = structuredClone(toRaw(props.projectData))

  if (projectData.value.id) {
    setValues()
    isEdit.value = true
  }
  else {
    clearValues()
    isEdit.value = false
  }
})

const onInvalidSubmit = ({ values, errors, results }) => {
  console.log(values)
  console.log(errors)
  console.log(results)
}

const onFormSubmit = handleSubmit(async values => {
  if (loading.value) return
  loading.value = true

  var data = {
    projectName: values.projectName,
    projectLocation: values.projectLocation,
    projectDetails: values.projectDetails,
    stageId: values.stageId,
    categoryId: values.categoryId,
    categoryOthersDescr: values.categoryOthersDescr,
    startDate: values.startDate,
    id: kawasanData.value.id,
  }

  await projectStore.save(data).then(response => {
    if (response.data.ok) {
      alertStore.success('Project data saved', '')
      emit('update:isDialogVisible', false)
      emit('submit', true)
    }
    else {
      throw response.data.message
    }
  }).catch(error => {
    console.log(error)
    alertStore.error('Error', error.response.data.message)
  }).finally(() => {
    loading.value = false
  })
}, onInvalidSubmit)

const onFormReset = () => {
  kawasanData.value = structuredClone(toRaw(props.kawasanData))
  emit('update:isDialogVisible', false)
}

const dialogModelValueUpdate = val => {
  emit('update:isDialogVisible', val)
}

const closeMe = () => {
  emit('update:isDialogVisible', false)
}
</script>

<template>
    <Dialog v-model:visible="props.isDialogVisible" :modal="true" :style="{ width: '600px' }" :header="props.dialogTitle">
        <Fluid>
            <form @submit="onFormSubmit">
                <div class="card flex flex-col gap-4">
                    <div class="flex flex-col gap-2">
                        <label for="projectId">Project Id</label>
                        <InputText v-if="isEdit" id="projectId" type="text" v-model="id" readonly />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="projectName">Project Name</label>
                        <Textarea id="projectName" type="text" v-model="projectName" />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="projectLocation">Project Location</label>
                        <Textarea id="projectLocation" v-model="projectLocation" />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="projectDetails">Project Details</label>
                        <Textarea id="projectDetails" v-model="projectDetails" />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="stageId">Stage</label>
                        <Dropdown id="stageId" v-model="stageId" :options="stageSelectList" optionLabel="name" />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="categoryId">Category</label>
                        <div v-for="category in categorySelectList" :key="category.id" class="flex items-center gap-2">
                            <RadioButton v-model="categoryId" :inputId="category.id" name="categoryId" :value="category.name" />
                            <label :for="category.id">{{ category.name }}</label>
                        </div>
                        <InputText v-if="categoryId===4" id="categoryOthersDescr" type="text" v-model="categoryOthersDescr" />
                    </div>
                    <div class="flex flex-col gap-2">
                        <label for="startDate" required>Start Date (yyyy-mm-dd)</label>
                        <DatePicker id="startDate" v-model="startDate" dateFormat="yy-mm-dd" />
                    </div>
                </div>
            </form>
        </Fluid>
        <template #footer>
            <div class="flex justify-end gap-4">
                <Button label="Cancel" icon="pi pi-times" severity="danger" @click="closeMe" />
                <Button label="Save" icon="pi pi-check" severity="success" type="submit" :disabled="isSubmitting || isDisabled" :loading="isSubmitting" />
            </div>
        </template>
    </Dialog>
</template>
