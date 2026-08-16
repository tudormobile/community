<script setup lang="ts">
import { computed, ref } from 'vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
import Calendar from '@/components/Calendar.vue'
import EventList from '@/components/EventList.vue'
import UpcomingEvent from '@/components/UpcomingEvent.vue'
import calendarIcon from '@/assets/icons/calendar_month.svg'
import listIcon from '@/assets/icons/list.svg'
import allEventsData from '@/assets/events.json'
import type { CalendarEvent } from '@/types/calendarEvent'
import type { LocationData } from '@/types/location'

type CalendarEventAsset = Omit<CalendarEvent, 'start' | 'end'> & {
  start: string
  end: string
  location?: LocationData
}

const allEvents = ref<CalendarEvent[]>(
  (allEventsData as CalendarEventAsset[]).map((event) => ({
    ...event,
    start: new Date(event.start),
    end: new Date(event.end),
  }))
)
const viewMode = ref<'first' | 'second'>('first')

const currentOrNextEvent = computed<CalendarEvent | null>(() => {
  const now = new Date()
  const today = allEvents.value.filter((event) => {
    return event.start.getFullYear() === now.getFullYear()
      && event.start.getMonth() === now.getMonth()
      && event.start.getDate() === now.getDate()
  }).sort((first, second) => first.start.getTime() - second.start.getTime())[0]

  if (today) return today

  return allEvents.value
    .filter((event) => event.start >= now)
    .sort((first, second) => first.start.getTime() - second.start.getTime())[0] ?? null
})

</script>
<template>
  <main class="events">
    <ItemsToggle v-model="viewMode" first-label="Calendar" second-label="Events" :first-icon="calendarIcon" :second-icon="listIcon" />
    <section class="events-content">
      <template v-if="viewMode === 'first'">
        <Calendar :events="allEvents" />
        <UpcomingEvent :event="currentOrNextEvent" />
      </template>
      <EventList v-else :events="allEvents" />
      <!--       TODO: add event details view/modal when an event is selected "EventList :events="allEvents" @select="selectedEvent = $event"
 -->
    </section>
  </main>
</template>

<style>
.events {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.events-content {
  flex: 1;
  overflow-y: auto;
  min-height: 0;
}
</style>
