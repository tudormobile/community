<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import allLocations from '@/assets/locations.json'
import type { LocationData } from '@/types/location'
import { createNearestLocationLookup } from '@/utils/nearestLocation'

type Coordinates = {
    lat: number
    lng: number
}

const locations = allLocations as LocationData[]
const nearestLocationLookup = createNearestLocationLookup(locations)

const isLocating = ref(false)
const locationError = ref('')
const currentLocation = ref<Coordinates | null>(null)

const nearestMatch = computed(() => {
    if (!currentLocation.value) {
        return null
    }

    return nearestLocationLookup.findNearest(currentLocation.value.lat, currentLocation.value.lng)
})

const formattedDistance = computed(() => {
    if (!nearestMatch.value) {
        return ''
    }

    const miles = nearestMatch.value.distanceMeters * 0.000621371
    return `${nearestMatch.value.distanceMeters.toFixed(0)} m (${miles.toFixed(2)} mi)`
})

function findCurrentLocation() {
    if (!navigator.geolocation) {
        locationError.value = 'Geolocation is not supported by this browser.'
        return
    }

    // testing with fixed location (1 Brittany Oaks, Clifton Park, NY)
    // currentLocation.value = { lat: 42.8458789, lng: -73.8291607 }
    // isLocating.value = false
    // return

    isLocating.value = true
    locationError.value = ''

    navigator.geolocation.getCurrentPosition(
        (position) => {
            currentLocation.value = {
                lat: position.coords.latitude,
                lng: position.coords.longitude,
            }
            isLocating.value = false
        },
        (error) => {
            locationError.value = error.message || 'Unable to retrieve your location.'
            isLocating.value = false
        },
        {
            enableHighAccuracy: true,
            timeout: 15000,
            maximumAge: 0,
        },
    )
}

onMounted(() => {
    findCurrentLocation()
})
</script>

<template>
    <section class="address">
        <h1>Nearest Matching Location</h1>

        <p v-if="isLocating">Getting your current location...</p>
        <p v-else-if="locationError" class="error">{{ locationError }}</p>

        <template v-else-if="nearestMatch && currentLocation">
            <p><strong>Closest Address:</strong> {{ nearestMatch.location.adr }}</p>
            <p><strong>Approximate Distance:</strong> {{ formattedDistance }}</p>
            <p>
                <strong>Current Location:</strong>
                {{ currentLocation.lat.toFixed(6) }}, {{ currentLocation.lng.toFixed(6) }}
            </p>
            <p>
                <strong>Matched Location:</strong>
                {{ nearestMatch.location.lat.toFixed(6) }}, {{ nearestMatch.location.lng.toFixed(6) }}
            </p>
        </template>

        <button type="button" class="refresh" @click="findCurrentLocation">Refresh Location</button>
    </section>
</template>
<style scoped>
.address {
    display: grid;
    gap: 0.5rem;
}

.error {
    color: #b42318;
}

.refresh {
    width: fit-content;
    padding: 0.5rem 0.75rem;
    border: 1px solid #b5bcc7;
    border-radius: 0.4rem;
    background: #fff;
    cursor: pointer;
}
</style>