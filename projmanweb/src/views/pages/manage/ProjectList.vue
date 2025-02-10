<script setup>
import { useAlertStore } from '@/stores';
import { useProjectStore } from '@/stores/project.store';
import { onMounted, ref } from 'vue';

import ProjectStage from '@/components/ProjectStage.vue';
import ProjectEditDialog from '@/views/dialog/ProjectEditDialog.vue';

const alertStore = useAlertStore();
const projectStore = useProjectStore();

const list = ref(null);
const rowTotal = ref(0);
const loading = ref(false);
const search = ref('');
const dtParams = ref({
    page: 1,
    pageSize: 10,
    sortField: 'Id',
    sortOrder: 1
});

const fetchList = () => {
    if (loading.value) return;

    let params = {
        ...dtParams.value,
        search: search.value
    };

    loading.value = true;
    projectStore
        .fetchPagedList(params)
        .then((response) => {
            let data = response.data;
            if (data.ok) {
                list.value = data.data;
                rowTotal.value = data.total;

                // let p = data.params;
                // options.value = p.options;
                // search.value = p.options.search;

                // totalPages.value = Math.ceil(totalRows.value / p.options.itemsPerPage);

                // nextTick(() => markSearch());
            } else {
                throw(data.message);
            }
        })
        .catch((error) => {
            console.log(error);
            alertStore.error('Error', error.response.data.message);
        })
        .finally(() => {
            loading.value = false;
        });
};

onMounted(() => {
    fetchList();
});

const isEditDialogVisible = ref(false)
const projectData = ref({})
const dialogTitle = ref('Add Project')

const addProject = () => {
  if (loading.value) return

    dialogTitle.value = 'Add Project';

  projectData.value = {
    projectName: '',
    projectLocation: '',
    projectDetails: '',
    stageId: 1,
    catgeoryId: 1,
    categoryOthersDescr: '',
    startDate: '',
    id: 0,
  }

  isEditDialogVisible.value = true
}

const editProject = id => {
  if (loading.value) return;

    dialogTitle.value = 'Edit Project';

  if (id) {
    projectStore.fetch(id).then(response => {
      var data = response.data
      if (data.ok) {
        projectData.value = data.row
        isEditDialogVisible.value = true
      }
    }). catch(error => {
      console.log(error)
      var message = error.response.data.message
      alertStore.error('Error fetching data', message)
    })
  }
}
</script>

<template>
    <div class="card">
        <div class="font-semibold text-xl mb-4">Construction Projects</div>
        <DataTable lazy ref="dt" :value="list" dataKey="id" :loading="loading"
            paginator :rows="10" :totalRecords="rowTotal"
            sortMode="single" :sortOrder="dtParams.sortOrder" :sortField="dtParams.sortField"
            rowHover stripedRows showGridlines>
            <template #header>
                <div class="flex justify-between">
                    <IconField>
                        <InputIcon>
                            <i class="pi pi-search" />
                        </InputIcon>
                        <InputText v-model="search" placeholder="Keyword Search" />
                    </IconField>
                </div>
            </template>
            <template #empty> No project found. </template>
            <template #loading> Loading projects data. Please wait. </template>

            <Column field="id" header="Project Id" sortable>
                <template #body="{ data }">
                    {{ data.id }}
                </template>
            </Column>
            <Column field="projectName" header="Project Name" sortable>
                <template #body="{ data }">
                    {{ data.projectName }}
                </template>
            </Column>
            <Column field="stageName" header="Stage" sortable>
                <template #body="{ data }">
                    <ProjectStage :stageId="data.stageId" :stageName="data.stageName" />
                </template>
            </Column>
            <Column field="categoryDescr" header="Category" sortable>
                <template #body="{ data }">
                    {{ data.categoryDescr }}
                </template>
            </Column>
            <Column field="startDate" header="Start Date" sortable>
                <template #body="{ data }">
                    {{ data.startDateString }}
                </template>
            </Column>
            <Column style="width: 2rem">
                <template #body="slotProps">
                    <div style="white-space: nowrap;">
                        <Button outlined icon="pi pi-pencil"
                            class="p-button-rounded p-button-success mr-2"
                            @click="editProject(slotProps.data.id)" />
                    </div>
                </template>
            </Column>
        </DataTable>
    </div>

    <ProjectEditDialog v-model:isDialogVisible="isEditDialogVisible" :dialog-title="dialogTitle" :projectData="projectData" />
</template>
