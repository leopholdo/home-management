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
    <div
      v-if="!list?.items.length && !isLoading"
      class="d-flex flex-column align-center justify-center flex-grow-1 ga-2">
      <v-icon
        size="50"
        color="primary">
        mdi-cart-off
      </v-icon>
      <div>
        <p class="text-body-large text-center mb-0">Sua lista de compras está vazia</p>
        <p class="text-body-large text-medium-emphasis text-center my-0">
          Comece adicionando itens apertando no botão abaixo
        </p>
      </div>
    </div>
    <TransitionGroup
      v-else
      name="shopping-list"
      tag="div"
      class="bg-surface">
      <div
        v-for="item in orderedItems"
        :key="item.id"
        class="swipe-wrapper">
        <div class="swipe-action-delete">
          <v-btn
            icon="mdi-delete"
            color="error"
            variant="flat"
            @click="removeItem(item)" />
        </div>
        <div
          class="swipe-content"
          :style="{
            transform: `translateX(${item.offset || 0}px)`,
          }"
          v-touch="{
            start: (e) => onTouchStart(e, item),
            move: (e) => onTouchMove(e, item),
            end: () => onTouchEnd(item),
          }">
          <v-list-item
            class="py-2 bg-surface"
            :class="{ 'complete-effect': item.isCompleting }">
            <template #prepend>
              <v-btn
                :icon="
                  item.isCompleted || item.isCompleting ? 'mdi-check-circle' : 'mdi-circle-outline'
                "
                :color="item.isCompleted || item.isCompleting ? 'success' : 'primary'"
                variant="text"
                @click="changeItemStatus(item)" />
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

                <span
                  :class="{
                    'text-disabled': item.isCompleted || item.isCompleting,
                  }">
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
        </div>
      </div>
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
  const touchStartX = ref(0)

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
    // close all opened rows
    closeAllItems()

    // force reset current item
    item.offset = 0

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

  function removeItem(item: ShoppingListItem) {
    list.value!.items = list.value!.items.filter((i) => i.id !== item.id)

    queueRemove(item.id)

    scheduleSync()
  }

  function onListUpdated(updatedItems: Record<ShoppingListItem>[]) {
    if (list.value) {
      list.value.items = updatedItems
    }
  }

  function closeAllItems() {
    list.value?.items.forEach((item) => {
      item.offset = 0
    })
  }

  function onTouchStart(e: TouchEvent, item: ShoppingListItem) {
    closeAllItems()

    touchStartX.value = e.originalEvent.touches[0].clientX
  }

  function onTouchMove(e: any, item: ShoppingListItem) {
    const currentX = e.originalEvent.touches[0].clientX

    const diff = currentX - touchStartX.value

    // only swipe left
    if (diff < 0) {
      item.offset = Math.max(diff, -90)
    }
  }

  function onTouchEnd(item: ShoppingListItem) {
    if ((item.offset || 0) < -45) {
      item.offset = -90
    } else {
      item.offset = 0
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

  .shopping-list-move {
    transition: transform 0.35s ease;
  }

  .shopping-list-enter-active,
  .shopping-list-leave-active {
    transition: all 0.35s ease;
  }

  .shopping-list-leave-active {
    position: absolute;
    width: 100%;
  }

  .swipe-wrapper {
    position: relative;
    overflow: hidden;
  }

  .swipe-content {
    position: relative;
    z-index: 2;

    will-change: transform;

    transition: transform 0.2s ease;

    touch-action: pan-y;

    background: rgb(var(--v-theme-surface));
  }

  .swipe-action-delete {
    position: absolute;

    top: 0;
    right: 0;
    bottom: 0;

    width: 90px;

    display: flex;
    align-items: center;
    justify-content: center;

    background: rgb(var(--v-theme-error));
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
