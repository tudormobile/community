<script setup lang="ts">
defineProps<{
  name: string
  presentCount: number
  totalCount: number
  status: 'all' | 'some' | 'none'
}>()

defineEmits<{
  (e: 'reset-group'): void
}>()
</script>

<template>
  <header class="travel-group-header">
    <div class="group-main">
      <span class="status-dot" :class="status" aria-hidden="true"></span>
      <h2>{{ name }}</h2>
      <p class="travel-group-total">{{ presentCount }} / {{ totalCount }}</p>
    </div>

    <button type="button" class="group-reset-button" @click="$emit('reset-group')">Reset Group</button>
  </header>
</template>

<style scoped>
.travel-group-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 0.3rem 0;
}

.group-main {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  min-width: 0;
}

.travel-group-total {
  color: var(--text-muted);
  font-weight: 600;
}

.status-dot {
  width: 0.62rem;
  height: 0.62rem;
  border-radius: 999px;
  display: inline-block;
  flex-shrink: 0;
}

.status-dot.none {
  background: #8f9aae;
}

.status-dot.some {
  background: #d9a100;
}

.status-dot.all {
  background: #1e8a4c;
}

.group-reset-button {
  min-height: 2.1rem;
  padding: 0.4rem 0.65rem;
  font-size: 0.85rem;
  flex-shrink: 0;
}
</style>