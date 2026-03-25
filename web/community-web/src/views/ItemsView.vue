<script setup lang="ts">
import { ref } from 'vue'
import configData from '@/assets/config.json'
import ItemsList from '@/components/ItemsList.vue'
import ItemsMap from '@/components/ItemsMap.vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
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

    <ItemsToggle v-model="viewMode" first-label="Map" second-label="List" />

    <ItemsMap v-if="viewMode === 'first'" :markers="locationMarkers" />
    <ItemsList v-else :markers="locationMarkers" />
  </section>
</template>

<style scoped>
.items-view {
  display: grid;
  gap: 1rem;
}

p {
  margin: 0;
}
</style>