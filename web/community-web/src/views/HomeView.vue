<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { ApiResponse, ServiceVersion } from '@/types/api'
import ItemsList from '@/components/ItemsList.vue'

const status = ref<ServiceVersion | null>(null)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    const additionalHeaders: Record<string, string> = JSON.parse(
      import.meta.env.VITE_API_ADDITIONAL_HEADERS
    )
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/status`, {
      headers: {
        [import.meta.env.VITE_API_KEY_NAME]: import.meta.env.VITE_API_KEY_VALUE,
        ...additionalHeaders,
      },
    })
    if (!response.ok) {
      error.value = `Request failed: ${response.status} ${response.statusText}`
      return
    }
    const result: ApiResponse<ServiceVersion> = await response.json()
    if (result.isSuccess) {
      status.value = result.data as ServiceVersion
    } else {
      error.value = result.data as string
    }
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Unknown error'
  }
})
</script>

<template>
  <main>
    <h1>Community Service Status</h1>
    <div v-if="error" class="error">{{ error }}</div>
    <dl v-else-if="status">
      <dt>Name</dt>
      <dd>{{ status.name }}</dd>
      <dt>Description</dt>
      <dd>{{ status.description }}</dd>
      <dt>Copyright</dt>
      <dd>{{ status.copyright }}</dd>
      <dt>Version</dt>
      <dd>{{ status.version }}</dd>
    </dl>
    <p v-else>Loading...</p>
    
  </main>
</template>
