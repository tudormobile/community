<script setup lang="ts">
defineProps<{
  name: string
  present: boolean
}>()

const emit = defineEmits<{
  (e: 'update:present', value: boolean): void
}>()

function onChange(event: Event) {
  const target = event.target as HTMLInputElement | null
  emit('update:present', Boolean(target?.checked))
}
</script>

<template>
  <label class="member-row">
    <input type="checkbox" class="member-checkbox" :checked="present" @change="onChange" />
    <span class="member-name">{{ name }}</span>
  </label>
</template>

<style scoped>
.member-row {
  display: flex;
  align-items: center;
  gap: 0.55rem;
  padding: 0.35rem 0.25rem;
  border-radius: var(--radius-sm);
  background: #ffffff;
  border: 1px solid var(--line);
}

.member-checkbox {
  width: 1rem;
  height: 1rem;
  accent-color: #1e8a4c;
  margin: 0;
}

.member-name {
  color: var(--text);
  font-size: 0.95rem;
}
</style>