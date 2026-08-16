<script setup lang="ts">
import { computed } from 'vue'
import type { CalendarEvent } from '@/types/calendarEvent'

const props = defineProps<{
  event: CalendarEvent | null
}>()

const isToday = computed(() => {
  if (!props.event) return false

  const today = new Date()
  const eventDate = props.event.start
  return eventDate.getFullYear() === today.getFullYear()
    && eventDate.getMonth() === today.getMonth()
    && eventDate.getDate() === today.getDate()
})

function formatDate(date: Date) {
  return date.toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' })
}

function formatTime(date: Date) {
  return date.toLocaleTimeString(undefined, { hour: 'numeric', minute: '2-digit' })
}
</script>

<template>
  <article v-if="props.event" class="upcoming-event">
    <p class="event-label">{{ isToday ? "Today's event" : 'Next event' }}</p>
    <h2>{{ props.event.title }}</h2>
    <p class="event-meta">
      {{ formatDate(props.event.start) }} · {{ formatTime(props.event.start) }}
      <span v-if="props.event.location"> · {{ props.event.location.adr }}</span>
    </p>
    <p v-if="props.event.description" class="event-description">{{ props.event.description }}</p>
  </article>
</template>

<style scoped>
.upcoming-event {
  margin: 0 0.75rem 0.75rem;
  padding: 0.85rem 1rem;
  border: 1px solid var(--line);
  border-radius: var(--radius-sm);
  background: var(--surface-soft);
}

.event-label {
  margin: 0;
  color: var(--text-muted);
  font-size: 0.72rem;
  font-weight: 600;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

h2 {
  margin: 0.2rem 0 0;
  color: var(--text-strong);
  font-size: 1rem;
}

.event-meta {
  margin: 0.2rem 0 0;
  color: var(--text-muted);
  font-size: 0.78rem;
}

.event-description {
  margin: 0.45rem 0 0;
  color: var(--text);
  font-size: 0.82rem;
  line-height: 1.4;
}
</style>