<script setup lang="ts">
type ListMarker = {
  id: string | number
  lat: number
  lng: number
  title?: string
  description?: string
}

defineProps<{
  markers: ListMarker[]
}>()
</script>

<template>
  <section class="items-list">
    <h2>Items List</h2>

    <ul v-if="markers.length" class="marker-list">
      <li v-for="marker in markers" :key="marker.id" class="marker-row">
        <h3>{{ marker.title || `Marker ${marker.id}` }}</h3>
        <p v-if="marker.description">{{ marker.description }}</p>
        <p>{{ marker.lat.toFixed(6) }}, {{ marker.lng.toFixed(6) }}</p>
      </li>
    </ul>

    <p v-else class="empty-state">No markers available.</p>
  </section>
</template>

<style scoped>
.items-list {
  display: grid;
  gap: 0.75rem;
}

h2,
h3 {
  margin: 0;
}

.marker-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: grid;
  gap: 0.75rem;
}

.marker-row {
  padding: 0.75rem;
  border: 1px solid #d0d5dd;
  border-radius: 0.5rem;
  background: #ffffff;
}

.marker-row p {
  margin: 0.35rem 0 0;
}

.empty-state {
  margin: 0;
  color: #667085;
}
</style>