<script setup lang="ts">
import { onMounted, ref } from 'vue'
import configData from '@/assets/config.json'
import allLocations from '@/assets/locations.json'
import allMarkers from '@/assets/markers.json'
import ItemsList from '@/components/ItemsList.vue'
import ItemsMap from '@/components/ItemsMap.vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
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