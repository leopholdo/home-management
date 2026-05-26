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
    <TransitionGroup
      name="shopping-list"
      tag="v-list"
      class="bg-surface">
      <v-list-item
        class="py-2"
        :class="{ 'complete-effect': item.isCompleting }"
        v-for="item in orderedItems"
        :key="item.id">
        <template #prepend>
          <v-icon-btn
            :icon="
              item.isCompleted || item.isCompleting ? 'mdi-check-circle' : 'mdi-circle-outline'
            "
            :color="item.isCompleted || item.isCompleting ? 'success' : 'primary'"
            variant="text"
            @click="changeItemStatus(item)"></v-icon-btn>
        </template>

        <v-list-item-title>
          {{ item.name }}
        </v-list-item-title>

        <template #append>
          <div class="d-flex align-center ga-2">
            <v-btn
              :disabled="item.quantity <= 0 || item.isCompleted || item.isCompleting"
              icon="mdi-minus"
              size="x-small"
              variant="text"
              @click.stop="decrementItem(item)" />

            <span :class="{ 'text-disabled': item.isCompleted || item.isCompleting }">
              {{ item.quantity }}
            </span>

            <v-btn
              :disabled="item.isCompleted || item.isCompleting"
              icon="mdi-plus"
              size="x-small"
              variant="text"
              @click.stop="addItem(item)" />
          </div>
        </template>
      </v-list-item>
    </TransitionGroup>
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

  const pendingBatch = ref<UpdateBatchShoppingListItemsRequest>({
    itemsToUpdate: [],
    itemsToRemove: [],
  })
  const syncTimeout = ref<NodeJS.Timeout | null>(null)

  const orderedItems = computed(() => {
    const items = list.value?.items

    if (!items?.length) return []

    return [...items].sort((a, b) => {
      const aCompleted = a.isCompleted && !a.isCompleting
      const bCompleted = b.isCompleted && !b.isCompleting

      return Number(aCompleted) - Number(bCompleted)
    })
  })

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

  function queueUpdate(item: ShoppingListItem) {
    pendingBatch.value.itemsToRemove = pendingBatch.value.itemsToRemove.filter(
      (i) => i.id !== item.id
    )

    const existing = pendingBatch.value.itemsToUpdate.find((i) => i.id === item.id)

    if (existing) {
      existing.quantity = item.quantity
    } else {
      pendingBatch.value.itemsToUpdate.push({
        id: item.id,
        quantity: item.quantity,
        isCompleted: item.isCompleted,
      })
    }
  }

  function queueRemove(itemId: string) {
    pendingBatch.value.itemsToUpdate = pendingBatch.value.itemsToUpdate.filter(
      (i) => i.id !== itemId
    )

    const exists = pendingBatch.value.itemsToRemove.some((i) => i.id === itemId)

    if (!exists) {
      pendingBatch.value.itemsToRemove.push({
        id: itemId,
      })
    }
  }

  function scheduleSync() {
    if (syncTimeout.value) {
      clearTimeout(syncTimeout.value)
    }

    syncTimeout.value = setTimeout(() => {
      syncBatch()
    }, 800)
  }

  async function syncBatch() {
    // clone
    const payload = JSON.parse(JSON.stringify(pendingBatch.value))

    const hasChanges = payload.itemsToUpdate.length || payload.itemsToRemove.length

    if (!hasChanges) return

    pendingBatch.value = {
      itemsToUpdate: [],
      itemsToRemove: [],
    }

    const response = await shoppingListService.updateItemsBatch(id.value, payload)

    if (!response.success) {
      snack.error(response.message)

      await getList()
    }
  }

  function changeItemStatus(item: Record<ShoppingListItem>) {
    if (item.isCompleted) {
      item.isCompleted = false

      queueUpdate(item)
      scheduleSync()

      return
    }

    // utilitário para aplicar animação de complete
    item.isCompleting = true

    setTimeout(() => {
      item.isCompleting = false
      item.isCompleted = true

      queueUpdate(item)
      scheduleSync()
    }, 700)
  }

  async function addItem(item: Record<ShoppingListItem>) {
    item.quantity++

    queueUpdate(item)

    scheduleSync()
  }

  async function decrementItem(item: Record<ShoppingListItem>) {
    item.quantity--

    if (item.quantity <= 0) {
      list.value!.items = list.value!.items.filter((i) => i.id !== item.id)

      queueRemove(item.id)
    } else {
      queueUpdate(item)
    }

    scheduleSync()
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

<style scoped>
  .shopping-list-move {
    transition: transform 0.35s ease;
  }

  .shopping-list-enter-active,
  .shopping-list-leave-active {
    transition: all 0.35s ease;
  }

  .complete-effect {
    animation: completeFlash 0.7s ease;
  }

  @keyframes completeFlash {
    0% {
      background-color: transparent;
    }

    50% {
      background-color: rgba(76, 175, 80, 0.35);
    }

    100% {
      background-color: transparent;
    }
  }
</style>
