<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { ServiceVersion } from '@/types/api'
import type { ApiResponse } from '@/types/api'

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
    error.value = 'Offline'
  }
})

</script>

<template>
    <div class="about-service">
        <div class="title">Service Information</div>
        <div class="item">
            <span>Service Status:</span>
            <span v-if="error" class="error">{{ error }}</span>
            <span v-if="status" class="status">Operational</span>
            <span v-if="!status && !error" class="query">Checking...</span>
        </div>
        <div class="item">
            <span>Name:</span>
            <span class="status">{{status?.name}}</span>
        </div>
        <div class="item">
            <span>Description:</span>
            <span class="status">{{status?.description}}</span>
        </div>
        <div class="item">
            <span>Copyright:</span>
            <span class="status">{{status?.copyright}}</span>
        </div>
        <div class="item">
            <span>Version:</span>
            <span class="status">{{status?.version}}</span>
        </div>
    </div>

</template>

<style scoped>
.about-service {
    display: grid;
    grid-template-columns: auto 1fr;
    column-gap: 1rem;
    row-gap: 0.1rem;
    align-items: baseline;
    padding-bottom: 2rem;
}
.title {
    font-size: 1rem;
    font-weight: bold;
    grid-column: 1 / -1;
}
.item {
    display: contents;
    font-size: 0.875rem;
}
.item > span {
    font-size: 0.875rem;
}

.status {
    color: var(--text);
    font-weight: bold;
}
.error {
    color: var(--text-error);
    font-weight: bold;
}
.query {
    color: var(--text-subtle);
    font-weight: bold;
    font-style: italic;
}
</style>