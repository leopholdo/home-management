<template>
  <v-dialog
    :model-value="model"
    transition="dialog-bottom-transition"
    fullscreen
    style="height: 100dvh">
    <v-card
      variant="flat"
      class="d-flex flex-column h-100">
      <v-card-title
        class="bg-surface-bright position-sticky top-0"
        style="z-index: 1">
        <div class="py-2">
          <v-text-field
            v-model="search"
            prepend-inner-icon="mdi-magnify"
            placeholder="Pesquisar ou incluir novo item"
            hide-details
            clearable
            density="comfortable"
            variant="solo-filled"
            rounded />
        </div>
      </v-card-title>
      <v-card-text class="d-flex flex-column pa-0">
        <v-list-item
          v-if="search"
          class="py-2">
          <template #prepend>
            <v-btn
              class="mr-4"
              icon="mdi-plus"
              size="small"
              variant="flat"
              @click="addNewItem(search)" />
          </template>

          <v-list-item-title>
            {{ search }}
          </v-list-item-title>

          <template #append>
            <div
              class="d-flex align-center ga-2"
              v-if="isAdded({ name: search })">
              <v-btn
                icon="mdi-minus"
                size="x-small"
                variant="text"
                @click.stop="decrementItem({ name: search })" />

              <span>
                {{ getQuantity({ name: search }) }}
              </span>

              <v-btn
                icon="mdi-plus"
                size="x-small"
                variant="text"
                @click.stop="addItem({ name: search })" />
            </div>
          </template>
        </v-list-item>

        <v-infinite-scroll
          :items="suggestions"
          :onLoad="load"
          class="mb-4"
          height="100%">
          <template v-slot:empty></template>
          <template
            v-for="suggestion in suggestions"
            :key="suggestion.id">
            <v-list-item
              class="py-2"
              @click="addItem(suggestion)">
              <template #prepend>
                <v-btn
                  class="mr-4"
                  :icon="isAdded(suggestion) ? 'mdi-check' : 'mdi-plus'"
                  :color="isAdded(suggestion) ? 'primary' : undefined"
                  size="small"
                  variant="flat"
                  @click.stop="addItem(suggestion)" />
              </template>

              <v-list-item-title>
                {{ suggestion.name }}
              </v-list-item-title>

              <template #append>
                <div
                  class="d-flex align-center ga-2"
                  v-if="isAdded(suggestion)">
                  <v-btn
                    icon="mdi-minus"
                    size="x-small"
                    variant="text"
                    @click.stop="decrementItem(suggestion)" />

                  <span>
                    {{ getQuantity(suggestion) }}
                  </span>

                  <v-btn
                    icon="mdi-plus"
                    size="x-small"
                    variant="text"
                    @click.stop="addItem(suggestion)" />
                </div>
              </template>
            </v-list-item>
          </template>

          <template #loading>
            <div class="d-flex justify-center py-4">
              <v-progress-circular
                indeterminate
                size="24" />
            </div>
          </template>
        </v-infinite-scroll>

        <div
          class="mt-auto pa-4 position-sticky bg-surface-bright"
          style="bottom: 0px">
          <v-btn
            class="w-100"
            color="primary"
            @click="finish">
            Finalizar
          </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
  import { useShoppingSuggestionService } from '@/services/shoppingSuggestionService'
  import { useShoppingListService } from '@/services/shoppingListService'
  import { useSnack } from '@/composables/useSnack'
  import debounce from 'lodash/debounce'

  const model = defineModel<boolean>()

  const props = defineProps<{
    listId: string
    originalItems: Record<ShoppingListItem>[]
  }>()

  const emit = defineEmits<{
    finished: []
    onListUpdated: Record<ShoppingListItem>[]
  }>()

  const shoppingSuggestionService = useShoppingSuggestionService()
  const shoppingListService = useShoppingListService()
  const snack = useSnack()

  const suggestions = ref<any[]>([])
  const originalItems = ref<Record<string, number>>({})
  const selectedItems = ref<Record<string, number>>({})

  const search = ref('')

  const page = ref(1)
  const limit = 20

  const loading = ref(false)
  const finishedLoading = ref(false)

  watch(
    search,
    debounce(async () => {
      reset()
      await fetchSuggestions()
    }, 300)
  )

  watch(model, async (opened) => {
    if (opened) {
      reset()
      init()
      await fetchSuggestions()
    }
  })

  const init = () => {
    originalItems.value = props.originalItems.reduce(
      (acc, item) => {
        acc[item.name] = {
          id: item.id,
          name: item.name,
          quantity: item.quantity,
        }

        return acc
      },
      {} as Record<string, { id: string; name: string; quantity: number }>
    )

    selectedItems.value = Object.fromEntries(
      Object.entries(originalItems.value).map(([key, value]) => [key, { ...value }])
    )
  }

  const reset = () => {
    page.value = 1
    finishedLoading.value = false
    suggestions.value = []
  }

  const fetchSuggestions = async () => {
    if (loading.value || finishedLoading.value) return

    loading.value = true

    try {
      const response = await shoppingSuggestionService.get({
        term: search.value,
        page: page.value,
        limit,
      })

      if (response.data.length < limit) finishedLoading.value = true

      suggestions.value.push(...response.data)

      page.value++
    } finally {
      loading.value = false
    }
  }

  const load = async ({ done }: any) => {
    await fetchSuggestions()

    done(finishedLoading.value ? 'empty' : 'ok')
  }

  const addNewItem = async (suggestionName: any) => {
    // api para adicionar nova sugestao
    let response = await shoppingSuggestionService.add({ name: suggestionName })
    console.log(response)

    addItem({ name: suggestionName })

    suggestions.value.pop({
      name: suggestionName,
    })
  }

  const addItem = async (suggestion: any) => {
    const existing = selectedItems.value[suggestion.name]

    if (existing) {
      existing.quantity++

      return
    }

    selectedItems.value[suggestion.name] = {
      name: suggestion.name,
      quantity: 1,
    }
  }

  const decrementItem = async (suggestion: any) => {
    const existing = selectedItems.value[suggestion.name]
    if (!existing) {
      return
    } else {
      if (existing.quantity === 1) {
        delete selectedItems.value[suggestion.name]
      } else {
        existing.quantity--
      }
    }
  }

  const getQuantity = (suggestion: any) => {
    return selectedItems.value[suggestion.name]?.quantity ?? 0
  }

  const isAdded = (suggestion: any) => {
    return getQuantity(suggestion) > 0
  }

  const finish = async () => {
    const itemsToAdd = []
    const itemsToUpdate = []
    const itemsToRemove = []

    for (const item of Object.values(selectedItems.value)) {
      console.log(item)
      const original = originalItems.value[item.name]

      // se nao tinha antes, é adicionado
      if (!original) {
        itemsToAdd.push({
          name: item.name,
          quantity: item.quantity,
        })

        continue
      }

      // se tinha antes, mas a quantidade mudou, é atualizado
      if (original.quantity !== item.quantity) {
        itemsToUpdate.push({
          id: original.id,
          quantity: item.quantity,
        })
      }
    }

    for (const original of Object.values(originalItems.value)) {
      if (!selectedItems.value[original.name]) {
        itemsToRemove.push({
          id: original.id,
        })
      }
    }

    if (itemsToAdd.length === 0 && itemsToUpdate.length === 0 && itemsToRemove.length === 0) {
      model.value = false
      return
    }

    const response = await shoppingListService.updateItemsBatch(props.listId, {
      itemsToAdd,
      itemsToUpdate,
      itemsToRemove,
    })

    let message = ''
    if (itemsToAdd.length > 0) {
      message +=
        itemsToAdd.length == 1 ? '1 item adicionado' : `${itemsToAdd.length} items adicionados`
    }
    if (itemsToUpdate.length > 0) {
      message +=
        itemsToUpdate.length == 1
          ? '1 item atualizado'
          : `${itemsToUpdate.length} items atualizados`
    }
    if (itemsToRemove.length > 0) {
      message +=
        itemsToRemove.length == 1 ? '1 item removido' : `${itemsToRemove.length} items removidos`
    }

    snack.success(`${message} com sucesso.`)

    model.value = false
    emit('onListUpdated', response.data)
  }
</script>
