<script setup lang="ts">
import { computed } from 'vue'
import type { CalendarEvent } from '@/types/calendarEvent'

const props = defineProps<{
  events: CalendarEvent[]
}>()

const attributes = computed(() =>
  props.events.map((event) => ({
    key: event.id,
    highlight: true,
    dates: { start: event.start, end: event.end },
    popover: { label: event.title },
    customData: event,
  }))
)
</script>

<template>
  <div class="calendar-wrapper">
    <VCalendar
      class="full-calendar"
      color="blue"
      :attributes="attributes"
    >
      <template #day-popover="{ attributes: attrs }">
        <div v-for="{ customData: evt } in attrs" :key="evt.id" class="event-popover">
          <strong>{{ evt.title }}</strong>
          <span v-if="evt.description" class="event-desc">{{ evt.description }}</span>
          <span v-if="evt.location" class="event-loc">{{ evt.location.adr }}</span>
        </div>
      </template>
    </VCalendar>
  </div>
</template>

<style scoped>
.calendar-wrapper {
  display: flex;
  justify-content: center;
  padding: 0.75rem;
  height: 100%;
}

.full-calendar {
  width: 100%;
  max-width: 480px;
}

.event-popover {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  padding: 0.25rem 0;
}

.event-desc,
.event-loc {
  font-size: 0.75rem;
  color: var(--text-muted);
}
</style>
