<script setup lang="ts">
import type { CalendarEvent } from '@/types/calendarEvent'
defineProps<{
  events: CalendarEvent[]
}>()

</script>
<template>
    <div class="calendar">
        <h1>This is the calendar view</h1>
    </div>
</template>
<style scoped>
</style>