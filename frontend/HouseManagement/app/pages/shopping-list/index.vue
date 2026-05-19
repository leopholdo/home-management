<template>
  <v-container
    class="d-flex overflow-hidden flex-column h-100 pa-0"
    style="gap: 16px">
    <div
      class="pt-4 px-4 d-flex align-center justify-space-between"
      style="margin-right: -10px">
      <span class="text-headline-small font-weight-semibold">Listas</span>
      <v-menu>
        <template v-slot:activator="{ props }">
          <v-icon-btn
            icon="mdi-dots-vertical"
            variant="text"
            v-bind="props"></v-icon-btn>
        </template>

        <v-list
          rounded="lg"
          slim
          density="compact">
          <v-list-item
            prepend-gap="10"
            title="Lixeira"
            @click="$router.push('/shopping-list/trash')">
            <template v-slot:prepend>
              <v-icon icon="mdi-trash-can"></v-icon>
            </template>
          </v-list-item>
        </v-list>
      </v-menu>
    </div>
    <SpinnerLoader v-model="isLoading.lists" />
    <div
      class="flex-grow-1 overflow-y-auto px-4 scroll-container"
      style="min-height: 0; margin-right: -6px">
      <v-row style="gap: 16px">
        <v-col
          cols="12"
          md="6"
          lg="4"
          v-for="list in shoppingLists"
          :key="list.id">
          <v-card
            @click="$router.push(`/shopping-list/${list.id}`)"
            class="pa-2"
            rounded="lg">
            <v-card-title class="d-flex align-center">
              {{ list.name }}
              <v-icon-btn
                class="ml-auto"
                style="margin-right: -15px"
                icon="mdi-dots-vertical"
                variant="text"
                @click="onClickListMenu(list)"></v-icon-btn>
            </v-card-title>
            <v-card-subtitle v-if="list.notes">{{ list.notes }}</v-card-subtitle>
            <v-card-text class="mt-2">
              <div class="d-flex align-center">
                <v-progress-linear
                  rounded="lg"
                  color="green-darken-3"
                  height="14"
                  :model-value="list.completedItems"
                  :max="list.totalItems" />
                <span class="ml-2">{{ list.completedItems }}/{{ list.totalItems }}</span>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <div class="mt-auto mb-4 px-4">
      <v-btn
        class="w-100"
        color="primary"
        @click="onClickCreateList">
        + Criar Lista
      </v-btn>
    </div>

    <v-bottom-sheet v-model="showListMenuBottomSheet">
      <div class="px-4 py-6 bg-surface rounded-t-xl">
        <v-list
          class="ma-0 pa-0"
          bg-color="surface-light"
          rounded="xl"
          style="border: 1px solid rgba(255, 255, 255, 0.05)">
          <v-list-item
            title="Renomear"
            @click="onClickRename">
            <template v-slot:prepend>
              <v-icon icon="mdi-pencil"></v-icon>
            </template>
          </v-list-item>
          <v-list-item
            title="Concluir itens"
            @click="sheet = false">
            <template v-slot:prepend>
              <v-icon icon="mdi-check"></v-icon>
            </template>
          </v-list-item>
          <v-divider></v-divider>
          <v-list-item
            class="text-red-lighten-2"
            title="Excluir"
            @click="onDelete">
            <template v-slot:prepend>
              <v-icon
                color="red-lighten-2"
                icon="mdi-trash-can"></v-icon>
            </template>
          </v-list-item>
        </v-list>
      </div>
    </v-bottom-sheet>

    <v-bottom-sheet v-model="showUpsertBottomSheet">
      <div class="px-4 py-6 d-flex flex-column align-center justify-center bg-surface rounded-t-xl">
        <span class="text-title-small font-weight-semibold">
          {{ isInsertingNewList ? 'Criar Lista' : 'Renomear Lista' }}
        </span>
        <v-form
          class="w-100 mt-4 d-flex flex-column ga-4"
          @submit.prevent="isInsertingNewList ? onSubmitNewList($event) : onSubmitRename($event)">
          <v-text-field
            ref="input-list-name"
            v-model="newListName"
            variant="outlined"
            color="primary"
            :rules="[rules.required()]" />
          <v-btn
            block
            height="46"
            rounded="xl"
            type="submit"
            color="success"
            :loading="isLoading.btnSubmit">
            Salvar
          </v-btn>
        </v-form>
      </div>
    </v-bottom-sheet>
  </v-container>
</template>

<script lang="ts" setup>
  import { useRules } from 'vuetify/labs/rules'
  import { useShoppingListService } from '@/services/shoppingListService'
  import { useSnack } from '@/composables/useSnack'
  import type { CreateShoppingListDto, ShoppingList } from '@home-management/types'

  const rules = useRules()
  const snack = useSnack()
  const shoppingListService = useShoppingListService()

  const inputListNameRef = useTemplateRef('input-list-name')
  const shoppingLists = ref([])
  const showListMenuBottomSheet = ref(false)
  const showUpsertBottomSheet = ref(false)
  const isInsertingNewList = ref(false)

  const isLoading = ref({
    lists: true,
    btnSubmit: false,
  })

  const newListName = ref('')
  const selectedList = ref<ShoppingList | null>(null)

  function getLists() {
    isLoading.value.lists = true
    shoppingListService.getAll().then((response) => {
      isLoading.value.lists = false
      if (response.success) {
        shoppingLists.value = response.data
      }
    })
  }

  function onClickListMenu(list: ShoppingList) {
    selectedList.value = list
    showListMenuBottomSheet.value = true
  }

  function onClickCreateList() {
    isInsertingNewList.value = true
    showUpsertBottomSheet.value = true
    newListName.value = selectedList.value?.name || ''

    nextTick(() => {
      inputListNameRef.value?.focus()
    })
  }

  function onClickRename() {
    showListMenuBottomSheet.value = false
    showUpsertBottomSheet.value = true
    newListName.value = selectedList.value?.name || ''

    nextTick(() => {
      inputListNameRef.value?.focus()
    })
  }

  async function onSubmitNewList(event) {
    await event
    isLoading.value.btnSubmit = true

    const response = await shoppingListService.create({
      name: newListName.value,
    })

    isLoading.value.btnSubmit = false
    if (!response.success) {
      snack.error('Erro ao criar lista')
      return
    }

    shoppingLists.value.unshift({
      ...response.data,
      completedItems: 0,
      totalItems: 0,
    })

    showUpsertBottomSheet.value = false
    snack.success('Lista criada com sucesso')
  }

  async function onSubmitRename(event) {
    await event
    if (!selectedList.value) return

    isLoading.value.btnSubmit = true

    shoppingListService
      .updateList(selectedList.value.id, {
        name: newListName.value,
        isCompleted: false,
      })
      .then((response) => {
        isLoading.value.btnSubmit = false
        if (!response.success) {
          snack.error('Erro ao renomear lista')
          return
        }

        const index = shoppingLists.value.findIndex((l) => l.id === selectedList.value?.id)
        if (index !== -1) {
          shoppingLists.value[index].name = newListName.value
        }
        showUpsertBottomSheet.value = false
        snack.success('Lista renomeada com sucesso')
      })
  }

  async function onDelete() {
    if (!selectedList.value) return

    isLoading.value.btnDelete = true

    const response = await shoppingListService.toggleDeleted(selectedList.value.id)

    if (response.success) {
      shoppingLists.value = shoppingLists.value.filter((l) => l.id !== selectedList.value?.id)
      showListMenuBottomSheet.value = false
      snack.success('Lista movida para lixeira com sucesso')
    } else {
      snack.error('Erro ao mover lista para lixeira')
    }
  }

  watch(showListMenuBottomSheet, (newValue) => {
    if (!newValue && !showUpsertBottomSheet.value) {
      selectedList.value = null
    }
  })
  watch(showUpsertBottomSheet, (newValue) => {
    if (!newValue) {
      nextTick(() => {
        newListName.value = ''
        selectedList.value = null
        isInsertingNewList.value = false
      })
    }
  })

  onMounted(() => {
    getLists()
  })
</script>

<style scoped>
  .scroll-container {
    overflow-y: auto;
    scrollbar-width: thin; /* Firefox */
    scrollbar-color: transparent transparent;
  }

  /* Chrome, Edge, Safari */
  .scroll-container::-webkit-scrollbar {
    width: 6px;
  }

  .scroll-container::-webkit-scrollbar-track {
    background: transparent;
  }

  .scroll-container::-webkit-scrollbar-thumb {
    background-color: transparent;
    border-radius: 999px;
    transition: background-color 0.2s;
  }

  /* Show scrollbar on hover */
  .scroll-container:hover::-webkit-scrollbar-thumb {
    background-color: rgba(255, 255, 255, 0.2);
  }

  /* Firefox hover */
  .scroll-container:hover {
    scrollbar-color: rgba(255, 255, 255, 0.2) transparent;
  }
</style>
