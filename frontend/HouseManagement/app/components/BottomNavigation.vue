<template>
  <v-bottom-navigation
    v-model="activeNavigation"
    color="primary"
    grow
    mandatory>
    <v-btn
      value="shopping-list"
      variant="plain">
      <v-icon>mdi-list-box</v-icon>
      Listas
    </v-btn>
  </v-bottom-navigation>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRoute } from 'vue-router'

  const route = useRoute()

  const activeNavigation = computed(() => {
    if (route.path.startsWith('/shopping-list')) {
      return 'shopping-list'
    }

    return null
  })
</script>
