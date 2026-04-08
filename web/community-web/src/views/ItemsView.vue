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
    <h1>Items</h1>
    <p>Switch between map and list views for the current items.</p>

    <ItemsToggle v-model="viewMode" first-label="Map" second-label="List" :first-icon="mapIcon" :second-icon="listIcon" />

    <ItemsMap v-if="viewMode === 'first'" :markers="locationMarkers" height="100%" />
    <ItemsList v-else :markers="locationMarkers" />
  </section>
</template>

<style scoped>
.items-view {
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: hidden;
  gap: 0.75rem;
  max-width: none;
  margin: 0;
  padding: 1rem 0.9rem;
}

p {
  margin: 0;
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
}
</style>