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
    <v-list>
      <v-list-item
        class="py-2"
        v-for="item in list?.items"
        :key="item.id">
        <template #prepend>
          <!-- <v-btn
            class="mr-4"
            :icon="isAdded(suggestion) ? 'mdi-check' : 'mdi-plus'"
            :color="isAdded(suggestion) ? 'primary' : undefined"
            size="small"
            variant="flat"
            @click.stop="addItem(suggestion)" /> -->
        </template>

        <v-list-item-title>
          {{ item.name }}
        </v-list-item-title>

        <template #append>
          <div class="d-flex align-center ga-2">
            <v-btn
              icon="mdi-minus"
              size="x-small"
              variant="text"
              @click.stop="decrementItem(item)" />

            <span>
              {{ item.quantity }}
            </span>

            <v-btn
              icon="mdi-plus"
              size="x-small"
              variant="text"
              @click.stop="addItem(item)" />
          </div>
        </template>
      </v-list-item>
    </v-list>
    <div class="mt-auto mb-4 px-4">
      <v-btn
        class="w-100"
        color="primary"
        @click="showAddItemModal = true">
        + Adicionar Item
      </v-btn>
    </div>
  </v-container>
  <SpinnerLoader v-model="isLoading" />
  <AddShoppingListItem
    v-model="showAddItemModal"
    :listId="id"
    :originalItems="list?.items || []"
    @onListUpdated="onListUpdated" />
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
  const showAddItemModal = ref(false)

  async function getList() {
    isLoading.value = true

    const response = await shoppingListService.getById(id.value)

    if (response.success) {
      list.value = response.data
    } else {
      snack.error(response.message)
    }

    isLoading.value = false
  }

  async function addItem(item: Record<ShoppingListItem>) {
    item.quantity = item.quantity + 1

    const response = await shoppingListService.updateItem(id.value, item.id, {
      name: item.name,
      quantity: item.quantity,
      isCompleted: false,
    })
  }

  async function decrementItem(item: Record<ShoppingListItem>) {
    if (item.quantity - 1 > 0) {
      item.quantity = item.quantity - 1

      const response = await shoppingListService.updateItem(id.value, item.id, {
        name: item.name,
        quantity: item.quantity,
        isCompleted: false,
      })
    } else {
      const response = await shoppingListService.removeItem(id.value, item.id)

      if (response.success) {
        list.value!.items = list.value!.items.filter((i) => i.id !== item.id)
      } else {
        snack.error(response.message)
      }
    }
  }

  function onListUpdated(updatedItems: Record<ShoppingListItem>[]) {
    if (list.value) {
      list.value.items = updatedItems
    }
  }

  onMounted(() => {
    getList()
  })
</script>
