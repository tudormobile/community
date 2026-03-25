<script setup lang="ts">
import { computed, shallowRef } from 'vue'
import { LMap, LMarker, LPopup, LTileLayer } from '@vue-leaflet/vue-leaflet'
import type { Icon, Map as LeafletMap } from 'leaflet'
import markerIcon2xUrl from 'leaflet/dist/images/marker-icon-2x.png'
import markerIconUrl from 'leaflet/dist/images/marker-icon.png'
import markerShadowUrl from 'leaflet/dist/images/marker-shadow.png'

type MapMarker = {
  id: string | number
  lat: number
  lng: number
  title?: string
  description?: string
}

const props = withDefaults(
  defineProps<{
    markers: MapMarker[]
    center?: [number, number]
    zoom?: number
    height?: string
  }>(),
  {
    zoom: 14,
    height: '32rem',
  },
)

const defaultCenter: [number, number] = [42.845879, -73.829161]
const markerIcon = shallowRef<Icon | null>(null)
let controlsInitialized = false

const mapOptions = {
  zoomControl: false,
}

const resolvedCenter = computed<[number, number]>(() => {
  if (props.center) {
    return props.center
  }

  const firstMarker = props.markers.at(0)

  if (firstMarker) {
    return [firstMarker.lat, firstMarker.lng]
  }

  return defaultCenter
})

const mapStyle = computed(() => ({
  height: props.height,
  width: '100%',
}))

async function onMapReady(map: LeafletMap) {
  if (controlsInitialized) {
    return
  }

  controlsInitialized = true

  const [{ control, Icon }, { LocateControl }] = await Promise.all([
    import('leaflet'),
    import('leaflet.locatecontrol'),
  ])

  markerIcon.value = new Icon({
    iconRetinaUrl: markerIcon2xUrl,
    iconUrl: markerIconUrl,
    shadowUrl: markerShadowUrl,
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41],
  })

  control.zoom({ position: 'bottomright' }).addTo(map)

  new LocateControl({
    position: 'bottomright',
    drawCircle: false,
    drawMarker: false,
    showCompass: false,
    showPopup: false,
    setView: 'always',
    flyTo: true,
    keepCurrentZoomLevel: false,
    initialZoomLevel: 17,
    strings: {
      title: 'Go to current location',
    },
    locateOptions: {
      enableHighAccuracy: true,
      maxZoom: 17,
    },
  }).addTo(map)
}
</script>

<template>
  <section class="items-map">
    <LMap
      :zoom="zoom"
      :center="resolvedCenter"
      :use-global-leaflet="false"
      :options="mapOptions"
      :style="mapStyle"
      @ready="onMapReady"
    >
      <LTileLayer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        layer-type="base"
        name="OpenStreetMap"
        attribution="&copy; OpenStreetMap contributors"
      />

      <LMarker
        v-if="markerIcon"
        v-for="marker in markers"
        :key="marker.id"
        :lat-lng="[marker.lat, marker.lng]"
        :icon="markerIcon"
      >
        <LPopup>
          <strong>{{ marker.title || `Marker ${marker.id}` }}</strong>
          <p v-if="marker.description">{{ marker.description }}</p>
          <p>{{ marker.lat.toFixed(6) }}, {{ marker.lng.toFixed(6) }}</p>
        </LPopup>
      </LMarker>
    </LMap>
  </section>
</template>

<style scoped>
.items-map {
  width: 100%;
}

:deep(.leaflet-container) {
  border: 1px solid #d0d5dd;
  border-radius: 0.75rem;
}

:deep(.leaflet-popup-content p) {
  margin: 0.35rem 0 0;
}

:deep(.leaflet-bottom.leaflet-right) {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
}

:deep(.leaflet-bottom.leaflet-right .leaflet-control) {
  width: auto;
}

:deep(.leaflet-control a) {
  padding: 0;
}

:deep(.leaflet-control-locate a) {
  display: flex;
  align-items: center;
  justify-content: center;
}
:deep(.leaflet-control-locate) {
  --locate-control-icon-color: #16a34a;
  --locate-control-active-color: #15803d;
  --locate-control-following-color: #22c55e;
}
</style>