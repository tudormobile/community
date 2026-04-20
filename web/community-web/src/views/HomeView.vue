<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import type { DbxResponse } from '@/types/api'
import type { Announcement } from '@/types/announcement'
import Notice from '@/components/Notice.vue'

import type { AppConfig } from '@/types/config'
import appConfig from '@/assets/config.json'

const announcementsId = '2b394650b277e000326337a5a2905f62bdae4c96cfbc319c5738a2b6a4c1c228'
const config = appConfig as AppConfig

const announcements = ref<Announcement[] | null>(null)
const error = ref<string | null>(null)

let pollTimer: ReturnType<typeof setInterval>

async function fetchAnnouncements() {
  try {
    const additionalHeaders: Record<string, string> = JSON.parse(
      import.meta.env.VITE_API_ADDITIONAL_HEADERS
    )
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/dbx/community/${announcementsId}`, {
      headers: {
        [import.meta.env.VITE_API_KEY_NAME]: import.meta.env.VITE_API_KEY_VALUE,
        ...additionalHeaders,
      },
    })
    if (!response.ok) {
      error.value = `Request failed: ${response.status} ${response.statusText}`
      return
    }
    const result: DbxResponse<Announcement[]> = await response.json()
    if (result.success && Array.isArray(result.data)) {
      announcements.value = result.data as Announcement[]
    } else {
      error.value = result.data as string
    }
  } catch (e) {
    error.value = e instanceof Error ? e.message : 'Unknown error'
  }
}

function onVisibilityChange() {
  if (document.visibilityState === 'visible') fetchAnnouncements()
}

onMounted(() => {
  fetchAnnouncements()
  pollTimer = setInterval(fetchAnnouncements, 30 * 60 * 1000)
  document.addEventListener('visibilitychange', onVisibilityChange)
})

onUnmounted(() => {
  clearInterval(pollTimer)
  document.removeEventListener('visibilitychange', onVisibilityChange)
})
</script>

<template>
  <main>
    <div>{{ config.description }}</div>
  </main>
  <div v-if="error" class="message">{{ error }}</div>
  <div v-else-if="announcements && announcements.length === 0" class="message">No announcements.</div>
  <div v-else-if="announcements && announcements?.length > 0">
    <ul>
      <li v-for="announcement in announcements" :key="announcement.id">
        <Notice :title="announcement.title" :content="announcement.content" :date="announcement.date" />
      </li>
    </ul>
  </div>
</template>
<style scoped>
.message {
  color: var(--text-subtle);
  font-size: 0.75rem;
  margin: 0.25rem 1.0rem;
  text-align: center;
}
ul {
  list-style: none;
  padding: 0;
  margin: 1rem 2rem;
}
</style>
