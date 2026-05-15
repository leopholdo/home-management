<template>
  <v-layout style="height: 56px;">
    <v-bottom-navigation
      v-model="activeNavigation"
      color="primary"
      grow mandatory>
      <v-btn value="shopping-list" variant="plain">
        <v-icon>mdi-list-box</v-icon>
        Listas
      </v-btn>
    </v-bottom-navigation>
  </v-layout>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRoute } from 'vue-router'

  const activeNavigation = ref()
  const route = useRoute()

  onMounted(() => {
    activeNavigation.value = route.name
  })
</script>