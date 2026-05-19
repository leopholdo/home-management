<template>
  <v-snackbar
    v-model="activeSnack.show"
    :timeout="activeSnack.timeout"
    :timer-color="activeSnack.color"
    timer="top"
    location="bottom"
    variant="tonal"
    @update:model-value="onClose">
    {{ activeSnack.message }}

    <template #actions>
      <v-btn
        icon="mdi-close"
        variant="text"
        @click="onClose" />
    </template>
  </v-snackbar>
</template>

<script setup lang="ts">
  const { activeSnack, close } = useSnack()

  function onClose() {
    close()
  }
</script>
