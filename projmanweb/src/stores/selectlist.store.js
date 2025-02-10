import api from '@/service/api'
import { defineStore } from 'pinia'

export const useSelectListStore = defineStore('SelectListStore', {
  id: 'select-list',
  actions: {
    fetchStageSelectList() {
      return api.get('/select/stage')
    },
    fetchCategorySelectList() {
      return api.get('/select/category')
    },
  },
})
