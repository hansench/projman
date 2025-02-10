<script setup>
import { useAlertStore } from "@/stores/alert.store";
import { useProjectStore } from "@/stores/project.store";
import { useSelectListStore } from "@/stores/selectlist.store";
import { useForm, useIsFormDirty, useIsFormValid } from 'vee-validate';
import { computed, defineProps, onMounted, ref, toRaw, watch } from 'vue';
import * as Yup from 'yup';
import moment from "moment";

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

const yesterday = new Date(Date.now() -86400000);

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
            then: () => Yup.string().label('Category Others Description')
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
            otherwise: () => Yup.date().label('Start Date')
                .min(yesterday, (data) => `${data.label} cannot be in the past`)
        })
})

const { defineField, setFieldValue, handleSubmit, errors, isSubmitting, resetForm } = useForm({
  validationSchema: schema,
})

const isDirty = useIsFormDirty();
const isValid = useIsFormValid();

const isEnableSubmit = computed(() => {
  return isDirty && isValid
})

const isDisabled = computed(() => {
  return !isEnableSubmit
})

const alertStore = useAlertStore();
const projectStore = useProjectStore();
const selectListStore = useSelectListStore();

const loading = ref(false);
const projectData = ref(structuredClone(toRaw(props.projectData)));
const isEdit = ref(true);
const stageSelectList = ref([]);
const categorySelectList = ref([]);

const fetchStageSelectList = () => {
  selectListStore.fetchStageSelectList(). then(response => {
    let data = response.data
    if (data.ok) {
      stageSelectList.value = response.data.data
    }
    else {
      console.log(data.message)
    }
  }). catch(error => {
    console.log(error)
  })
}

const fetchCategorySelectList = () => {
  selectListStore.fetchCategorySelectList(). then(response => {
    let data = response.data
    if (data.ok) {
      categorySelectList.value = response.data.data
    }
    else {
      console.log(data.message)
    }
  }). catch(error => {
    console.log(error)
  })
}

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

const formatDate = (date) => {
  if (date) {
    return moment(date).format("YYYY-MM-DD");
  }

  return "";
}

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
    startDate: formatDate(values.startDate),
    id: projectData.value.id,
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
  projectData.value = structuredClone(toRaw(props.projectData))
  emit('update:isDialogVisible', false)
}

const dialogModelValueUpdate = val => {
  emit('update:isDialogVisible', val)
}

// const closeMe = () => {
//   emit('update:isDialogVisible', false)
// }

const mindate = computed(() => {
  return stageId.value === 4 ? null : yesterday
})

const visible = computed({
  get: () => props.isDialogVisible,
  set: val => dialogModelValueUpdate(val)
})

onMounted(() => {
  fetchStageSelectList();
  fetchCategorySelectList();
})
</script>

<template>
    <Dialog v-model:visible="visible" :modal="true" :style="{ width: '600px' }" :header="props.dialogTitle">
        <form>
            <div class="card flex flex-col gap-4">
                <div class="flex flex-col gap-2">
                    <label for="projectId" class="font-bold ml-1">Project Id</label>
                    <InputText v-if="isEdit" id="projectId" type="text" v-model="id" readonly />
                </div>
                <div class="flex flex-col gap-2">
                    <label for="projectName" class="font-bold ml-1">Project Name</label>
                    <Textarea id="projectName" type="text" v-model="projectName" />
                    <Message v-if="errors.projectName" severity="error" size="small" variant="simple">{{ errors.projectName }}</Message>
                </div>
                <div class="flex flex-col gap-2">
                    <label for="projectLocation" class="font-bold ml-1">Project Location</label>
                    <Textarea id="projectLocation" v-model="projectLocation" />
                    <Message v-if="errors.projectLocation" severity="error" size="small" variant="simple">{{ errors.projectLocation }}</Message>
                </div>
                <div class="flex flex-col gap-2">
                    <label for="projectDetails" class="font-bold ml-1">Project Details</label>
                    <Textarea id="projectDetails" v-model="projectDetails" />
                    <Message v-if="errors.projectDetails" severity="error" size="small" variant="simple">{{ errors.projectDetails }}</Message>
                </div>
                <div class="flex flex-col gap-2">
                    <label for="stageId" class="font-bold ml-1">Stage</label>
                    <Select id="stageId" v-model="stageId" :options="stageSelectList" option-label="name" option-value="id" placeholder="Select Stage" />
                    <Message v-if="errors.stageId" severity="error" size="small" variant="simple">{{ errors.stageId }}</Message>
                </div>
                <div class="flex flex-col gap-2">
                    <label for="categoryId" class="font-bold ml-1">Category</label>
                    <div v-for="category in categorySelectList" :key="category.id" class="flex items-center gap-2">
                        <RadioButton v-model="categoryId" :inputId="category.name" name="categoryId" :value="category.id" />
                        <label :for="category.name">{{ category.name }}</label>
                    </div>
                    <InputText v-if="categoryId===4" id="categoryOthersDescr" type="text" v-model="categoryOthersDescr" />
                    <Message v-if="errors.categoryOthersDescr" severity="error" size="small" variant="simple">{{ errors.categoryOthersDescr }}</Message>
                </div>
                <div class="flex flex-col gap-2">
                    <label for="startDate" class="font-bold ml-1">Start Date (yyyy-mm-dd)</label>
                    <DatePicker id="startDate" v-model="startDate" dateFormat="yy-mm-dd" :min-date="mindate" show-icon />
                    <Message v-if="errors.startDate" severity="error" size="small" variant="simple">{{ errors.startDate }}</Message>
                </div>
            </div>
        </form>
        <template #footer>
            <div class="flex justify-end gap-4">
                <Button label="Cancel" icon="pi pi-times" severity="danger" @click="onFormReset" />
                <Button label="Save" icon="pi pi-check" severity="success" :disabled="isSubmitting || isDisabled" :loading="isSubmitting" @click="onFormSubmit" />
            </div>
        </template>
    </Dialog>
</template>
