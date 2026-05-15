<template>
  <h1>
    test
  </h1>
</template>

<script lang="ts" setup>
import { useShoppingListService } from '@/services/shoppingListService'

const shoppingListService = useShoppingListService()

onMounted(() => {
  shoppingListService.getAll().then((lists) => {
    console.log(lists)
  })
})
</script>