<script setup lang="ts">
import type { CalendarEvent } from '@/types/calendarEvent'

const props = defineProps<{
  events: CalendarEvent[]
}>()

const emit = defineEmits<{
  (e: 'select', event: CalendarEvent): void
}>()

function formatDate(date: Date) {
  return new Date(date).toLocaleDateString(undefined, { weekday: 'short', month: 'short', day: 'numeric', year: 'numeric' })
}
</script>

<template>
  <div class="event-list">
    <p class="event-count">{{ props.events.length ? `${props.events.length} event${props.events.length === 1 ? '' : 's'}` : 'No events.' }}</p>
    <ul class="event-items">
      <li
        v-for="event in props.events"
        :key="event.id"
        class="event-item"
        role="button"
        tabindex="0"
        @click="emit('select', event)"
        @keydown.enter="emit('select', event)"
        @keydown.space.prevent="emit('select', event)"
      >
        <div class="event-title">{{ event.title }}</div>
        <div class="event-meta">
          <span>{{ formatDate(event.start) }}</span>
          <span v-if="event.location" class="event-location">· {{ event.location.adr }}</span>
        </div>
        <p v-if="event.description" class="event-description">{{ event.description }}</p>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.event-list {
  display: flex;
  flex-direction: column;
  height: 100%;
  overflow: hidden;
}

.event-count {
  font-size: 0.72rem;
  color: var(--text-muted);
  margin: 0.5rem 1rem 0.25rem;
  text-align: center;
}

.event-items {
  list-style: none;
  margin: 0;
  padding: 0 0.75rem;
  overflow-y: auto;
  flex: 1;
}

.event-item {
  padding: 0.75rem 0.5rem;
  border-bottom: 1px solid var(--line);
  cursor: pointer;
  border-radius: var(--radius-sm);
  transition: background 0.15s ease;
}

.event-item:last-child {
  border-bottom: none;
}

.event-item:hover,
.event-item:focus-visible {
  background: var(--surface-soft);
  outline: none;
}

.event-title {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--text-strong);
}

.event-meta {
  font-size: 0.75rem;
  color: var(--text-muted);
  margin-top: 0.15rem;
}

.event-location {
  margin-left: 0.15rem;
}

.event-description {
  font-size: 0.8rem;
  color: var(--text);
  margin: 0.25rem 0 0;
  line-height: 1.4;
}
</style>
