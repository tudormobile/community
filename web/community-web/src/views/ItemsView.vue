<script setup lang="ts">
import { ref } from 'vue'
import configData from '@/assets/config.json'
import ItemsList from '@/components/ItemsList.vue'
import ItemsMap from '@/components/ItemsMap.vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
import mapIcon from '@/assets/map.svg'
import listIcon from '@/assets/list.svg'
import type { AppConfig } from '@/types/config'

const appConfig = configData as AppConfig
const viewMode = ref<'first' | 'second'>('first')

const locationMarkers = [
  {
    id: 'initial-home-marker',
    lat: appConfig.homeLocation.lat,
    lng: appConfig.homeLocation.lng,
    title: appConfig.homeLocation.adr,
    description: '',
  },
]
</script>

<template>
  <section class="items-view">
    <div class="toggle-wrapper">
      <ItemsToggle v-model="viewMode" first-label="Map" second-label="List" :first-icon="mapIcon" :second-icon="listIcon" />
    </div>

    <ItemsMap v-if="viewMode === 'first'" :markers="locationMarkers" height="100%" />
    <ItemsList v-else :markers="locationMarkers" />
  </section>
</template>

<style scoped>
.items-view {
  display: flex;
  flex-direction: column;
  position: fixed;
  top: calc(var(--top-bar-height) + env(safe-area-inset-top));
  left: 0;
  right: 0;
  bottom: 4.5rem;
  overflow: hidden;
  max-width: none;
  margin: 0;
  padding: 0;
  gap: 0;
}

p {
  margin: 0;
}

.toggle-wrapper {
  padding: 0.4rem 0;
  flex-shrink: 0;
}

:deep(.items-list) {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
}

:deep(.items-map) {
  flex: 1;
  min-height: 0;
  overflow: hidden;
  max-width: none;
  width: 100%;
  margin: 0;
}
</style>