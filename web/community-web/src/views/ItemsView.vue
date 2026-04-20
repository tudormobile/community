<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import configData from '@/assets/config.json'
import allLocations from '@/assets/locations.json'
import allMarkers from '@/assets/markers.json'
import ItemsList from '@/components/ItemsList.vue'
import ItemsMap from '@/components/ItemsMap.vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
import SearchBox from '@/components/SearchBox.vue'
import mapIcon from '@/assets/icons/map.svg'
import listIcon from '@/assets/icons/list.svg'
import type { AppConfig } from '@/types/config'
import type { LocationData } from '@/types/location'
import { createNearestLocationLookup } from '@/utils/nearestLocation'


interface LocationMarker {
  id: string | number
  title: string
  description: string
  lat: number
  lng: number
}

interface RawLocationMarker {
  id: string
  title: string
  description: string
  lat: number | string
  lng: number | string
}

const appConfig = configData as AppConfig
const viewMode = ref<'first' | 'second'>('first')
const searchTextValue = ref('')
const nearestLocationLookup = createNearestLocationLookup(allLocations as LocationData[])

function toNumber(value: number | string): number {
  if (typeof value === 'number') {
    return value
  }

  const parsed = Number.parseFloat(value)
  return Number.isFinite(parsed) ? parsed : Number.NaN
}

const normalizedMarkers: LocationMarker[] = (allMarkers as RawLocationMarker[])
  .map((marker) => ({
    ...marker,
    lat: toNumber(marker.lat),
    lng: toNumber(marker.lng),
  }))
  .filter((marker) => Number.isFinite(marker.lat) && Number.isFinite(marker.lng))

const locationMarkers = ref<LocationMarker[]>([
  {
    id: 'initial-home-marker',
    lat: appConfig.homeLocation.lat,
    lng: appConfig.homeLocation.lng,
    title: appConfig.homeLocation.adr,
    description: '',
  },
  ...normalizedMarkers,
])

const filteredMarkers = computed<LocationMarker[]>(() => {
  const normalizedQuery = searchTextValue.value.trim().toLowerCase()

  if (!normalizedQuery) {
    return locationMarkers.value
  }

  return locationMarkers.value.filter((marker) => {
    const title = marker.title?.toLowerCase() ?? ''
    const description = marker.description?.toLowerCase() ?? ''
    return title.includes(normalizedQuery) || description.includes(normalizedQuery)
  })
})

function appendAddressToTitle(marker: LocationMarker, address: string): string {
  const trimmedAddress = address.trim()
  const baseTitle = marker.title.trim()

  if (!trimmedAddress) {
    return baseTitle
  }

  if (!baseTitle) {
    return trimmedAddress
  }

  if (baseTitle.includes(trimmedAddress)) {
    return baseTitle
  }

  return `${baseTitle} - ${trimmedAddress}`
}

async function enrichMarkerTitlesInBackground() {
  // Skip the initial home marker at index 0.
  for (let i = 1; i < locationMarkers.value.length; i += 1) {
    const marker = locationMarkers.value[i]

    if (!marker) {
      continue
    }

    const nearest = nearestLocationLookup.findNearest(marker.lat, marker.lng)

    if (nearest?.location?.adr) {
      marker.title = appendAddressToTitle(marker, nearest.location.adr)
    }

    // Yield periodically so list/map remain responsive while enriching many records.
    if (i % 20 === 0) {
      await new Promise<void>((resolve) => setTimeout(resolve, 0))
    }
  }
}

onMounted(() => {
  void enrichMarkerTitlesInBackground()
})

function handleAddEvent() {
  // Placeholder for future event creation flow.
}
</script>

<template>
  <section class="items-view">
    <template v-if="viewMode === 'first'">
      <ItemsMap class="map-layer" :markers="filteredMarkers" height="100%" />

      <div class="overlay-controls">
        <div class="toggle-wrapper">
          <ItemsToggle v-model="viewMode" first-label="Map" second-label="List" :first-icon="mapIcon" :second-icon="listIcon" />
        </div>

        <div class="search-wrapper">
          <SearchBox v-model:search-text-value="searchTextValue" @add="handleAddEvent" />
        </div>
      </div>
    </template>

    <template v-else>
      <div class="toggle-wrapper">
        <ItemsToggle v-model="viewMode" first-label="Map" second-label="List" :first-icon="mapIcon" :second-icon="listIcon" />
      </div>

      <div class="search-wrapper">
        <SearchBox v-model:search-text-value="searchTextValue" @add="handleAddEvent" />
      </div>

      <ItemsList :markers="filteredMarkers" />
    </template>
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

.map-layer {
  position: absolute;
  inset: 0;
  z-index: 0;
}

.overlay-controls {
  position: absolute;
  inset: 0 auto auto 0;
  width: 100%;
  z-index: 1;
  pointer-events: none;
}

.overlay-controls > * {
  pointer-events: auto;
}

p {
  margin: 0;
}

.toggle-wrapper {
  padding: 0.4rem 0;
  flex-shrink: 0;
  display: flex;
  justify-content: center;
}

.search-wrapper {
  padding: 0 0.75rem 0.5rem;
  display: flex;
  justify-content: center;
  flex-shrink: 0;
}

:deep(.items-list) {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  max-width: none;
  border-radius: 0;
  border-left: none;
  border-right: none;
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