<script setup lang="ts">
import { onMounted, ref } from 'vue'
import CameraCapture from './components/CameraCapture.vue'
import ThumbnailGrid from './components/ThumbnailGrid.vue'
import { listThumbnails, type ThumbnailRecord } from './lib/thumbnailStore'

const thumbnails = ref<ThumbnailRecord[]>([])
const loading = ref(false)
const loadError = ref('')

const refreshThumbnails = async () => {
  loading.value = true
  loadError.value = ''

  try {
    thumbnails.value = await listThumbnails()
  } catch (error) {
    loadError.value =
      error instanceof Error ? error.message : 'Unable to load thumbnails from IndexedDB.'
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await refreshThumbnails()
})
</script>

<template>
  <main class="app-shell">
    <h1>Thumbnail Capture Prototype</h1>
    <p class="intro">
      Start the camera, frame the object in the overlay, then capture and save a 1:1 thumbnail.
    </p>

    <CameraCapture @saved="refreshThumbnails" />

    <section class="saved-list" aria-live="polite">
      <h2>Saved Thumbnails</h2>
      <p v-if="loading">Loading thumbnails...</p>
      <p v-else-if="loadError">{{ loadError }}</p>
      <ThumbnailGrid v-else :items="thumbnails" />
    </section>
  </main>
</template>

<style scoped>
.app-shell {
  max-width: 900px;
  margin: 0 auto;
  padding: 1rem;
  font-family: sans-serif;
}

.intro {
  margin-top: 0;
}

.saved-list {
  margin-top: 1rem;
}

h1,
h2 {
  margin-bottom: 0.5rem;
}
</style>
