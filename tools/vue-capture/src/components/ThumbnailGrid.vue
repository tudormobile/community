<script setup lang="ts">
import { computed, onBeforeUnmount } from 'vue'
import type { ThumbnailRecord } from '../lib/thumbnailStore'

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
    id: item.id,
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
      <li v-for="item in gridItems" :key="item.id" class="cell">
        <img :src="item.src" :alt="`Thumbnail ${item.id}`" :class="item.shape" loading="lazy" />
        <small>{{ toDisplayTime(item.createdAt) }}</small>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.grid-scroll {
  max-height: 50vh;
  overflow: auto;
  border: 1px solid #d8d8d8;
  padding: 0.5rem;
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
  background: #f1f1f1;
}

img.circle {
  border-radius: 50%;
}

small {
  font-size: 0.68rem;
}
</style>