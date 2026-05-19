<template>
  <v-container class="d-flex overflow-hidden flex-column h-100 pa-0">
    <div class="d-flex align-center bg-surface">
      <v-btn
        variant="text"
        icon="mdi-arrow-left"
        class="mr-2"
        @click="$router.push('/shopping-list')"></v-btn>
      <span class="text-title-medium font-weight-semibold">{{ list?.name }}</span>
    </div>

    <div class="mt-auto mb-4 px-4">
      <v-btn
        class="w-100"
        color="primary"
        @click="onClickCreateList">
        + Adicionar Item
      </v-btn>
    </div>
  </v-container>

  <v-dialog
    v-model="showAddItemModal"
    transition="dialog-bottom-transition"
    fullscreen>
    <v-card>
      <v-toolbar color="surface">
        <v-btn
          icon="mdi-arrow-left"
          @click="showAddItemModal = false"></v-btn>

        <v-toolbar-title class="ml-0 mr-4">
          <v-text-field
            autocomplete="off"
            label="Adicionar ou pesquisar item"
            variant="solo-filled"
            prepend-inner-icon="mdi-magnify"></v-text-field>
        </v-toolbar-title>
      </v-toolbar>

      <div></div>
    </v-card>
  </v-dialog>
  <SpinnerLoader v-model="isLoading" />
</template>

<script setup lang="ts">
  import { useShoppingListService } from '@/services/shoppingListService'
  import { useSnack } from '@/composables/useSnack'

  const shoppingListService = useShoppingListService()
  const route = useRoute()
  const snack = useSnack()

  const id = computed(() => route.params.id)

  const isLoading = ref(false)
  const list = ref<ShoppingList | null>(null)
  const showAddItemModal = ref(true)

  async function getList() {
    isLoading.value = true

    const response = await shoppingListService.getById(id.value)

    if (response.success) {
      list.value = response.data
      console.log(list.value)
    } else {
      snack.error(response.message)
    }

    isLoading.value = false
  }

  onMounted(() => {
    getList()
  })
</script>
