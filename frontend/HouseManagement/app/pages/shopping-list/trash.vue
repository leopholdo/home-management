<template>
  <v-container
    class="d-flex overflow-hidden flex-column h-100 pa-0"
    style="gap: 16px">
    <div
      class="pt-4 px-4 d-flex align-center"
      style="margin-right: -10px">
      <v-icon-btn
        icon="mdi-arrow-left"
        variant="text"
        @click="$router.back()"></v-icon-btn>
      <span class="text-headline-small font-weight-semibold">Lixeira</span>
    </div>
    <SpinnerLoader v-model="isLoading.lists" />
    <div
      v-if="shoppingLists.length > 0 && !isLoading.lists"
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
            class="pa-2"
            rounded="lg">
            <v-card-title class="d-flex align-center">
              {{ list.name }}
            </v-card-title>
            <v-card-subtitle v-if="list.notes">{{ list.notes }}</v-card-subtitle>
            <v-card-text class="mt-2 pb-0c">
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
            <v-card-actions>
              <div class="d-flex justify-end align-center w-100 ga-4">
                <v-btn
                  color="danger"
                  @click="onDelete(list)">
                  Excluir
                </v-btn>
                <v-btn
                  variant="tonal"
                  color="success"
                  @click="onRestore(list)">
                  Restaurar
                </v-btn>
              </div>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <div
      class="px-4 d-flex align-center justify-center flex-column h-100"
      v-else>
      <v-icon size="3rem">mdi-delete-empty</v-icon>
      <div class="text-center">
        <p>Nenhuma lista na lixeira.</p>
        <p class="text-medium-emphasis">
          Aqui você pode encontrar listas que foram excluídas, podendo restaurá-las ou apagá-las
          permanentemente.
        </p>
      </div>
    </div>
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

  const shoppingLists = ref([])

  const isLoading = ref({
    lists: true,
    btnDelete: false,
    btnRestore: false,
  })

  const selectedList = ref<ShoppingList | null>(null)

  function getLists() {
    isLoading.value.lists = true
    shoppingListService.getAll(true).then((response) => {
      isLoading.value.lists = false
      if (response.success) {
        shoppingLists.value = response.data
      }
    })
  }

  async function onDelete(list: ShoppingList) {
    isLoading.value.btnDelete = true

    const response = await shoppingListService.remove(list.id)

    if (response.success) {
      shoppingLists.value = shoppingLists.value.filter((l) => l.id !== list.id)
      snack.success('Lista excluída permanentemente com sucesso')
    } else {
      snack.error('Erro ao excluir lista')
    }
  }

  async function onRestore(list: ShoppingList) {
    isLoading.value.btnRestore = true

    const response = await shoppingListService.toggleDeleted(list.id)

    if (response.success) {
      shoppingLists.value = shoppingLists.value.filter((l) => l.id !== list.id)
      snack.success('Lista restaurada com sucesso')
    } else {
      snack.error('Erro ao restaurar lista')
    }
  }

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
