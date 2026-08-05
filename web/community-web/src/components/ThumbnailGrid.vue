<script setup lang="ts">
import { computed, onBeforeUnmount } from 'vue'
import type { ThumbnailRecord } from '@/lib/thumbnailStore'

const props = defineProps<{
  items: ThumbnailRecord[]
}>()

let previousUrls: string[] = []

const gridItems = computed(() => {
  for (const url of previousUrls) {
    URL.revokeObjectURL(url)
  }

  previousUrls = props.items.map((item) => URL.createObjectURL(item.blob))

  return props.items.map((item, index) => ({
    thumbnailId: item.thumbnailId,
    groupId: item.groupId,
    partyId: item.partyId,
    shape: item.shape,
    createdAt: item.createdAt,
    src: previousUrls[index],
  }))
})

onBeforeUnmount(() => {
  for (const url of previousUrls) {
    URL.revokeObjectURL(url)
  }
})

const toDisplayTime = (createdAt: number): string => new Date(createdAt).toLocaleString()
</script>

<template>
  <p v-if="items.length === 0">No thumbnails captured yet.</p>
  <div v-else class="grid-scroll">
    <ul class="grid" role="list">
      <li v-for="item in gridItems" :key="item.thumbnailId" class="cell">
        <img
          :src="item.src"
          :alt="`Thumbnail for group ${item.groupId}, party ${item.partyId}`"
          :class="item.shape"
          loading="lazy"
        />
        <small>{{ toDisplayTime(item.createdAt) }}</small>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.grid-scroll {
  max-height: 50vh;
  overflow: auto;
  border: 1px solid var(--line);
  border-radius: var(--radius-md);
  padding: 0.5rem;
  background: var(--surface-soft);
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(96px, 1fr));
  gap: 0.5rem;
  list-style: none;
  margin: 0;
  padding: 0;
}

.cell {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

img {
  width: 100%;
  aspect-ratio: 1 / 1;
  object-fit: cover;
  background: var(--surface);
}

img.circle {
  border-radius: 50%;
}

small {
  font-size: 0.68rem;
  color: var(--text-muted);
}
</style>
