<script setup lang="ts">
import type { CalendarEvent } from '@/types/calendarEvent'
defineProps<{
  events: CalendarEvent[]
}>()

</script>
<template>
    <div class="event-list">
        <h1>This is the event list view</h1>
    </div>
</template>
<style scoped></style>