<script setup lang="ts">
import { ref } from 'vue'
import ItemsToggle from '@/components/ItemsToggle.vue'
import Calendar from '@/components/Calendar.vue'
import EventList from '@/components/EventList.vue'
import calendarIcon from '@/assets/icons/calendar_month.svg'
import listIcon from '@/assets/icons/list.svg'
import type { CalendarEvent } from '@/types/calendarEvent'

const allEvents = ref<CalendarEvent[]>([])
const viewMode = ref<'first' | 'second'>('first')

</script>
<template>
  <main class="events">
    <ItemsToggle v-model="viewMode" first-label="Calendar" second-label="Events" :first-icon="calendarIcon" :second-icon="listIcon" />
    <section class="events-content">
      <Calendar v-if="viewMode === 'first'" :events="allEvents" />
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
