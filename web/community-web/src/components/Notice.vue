<script setup lang="ts">
import CommentIcon from '@/assets/icons/comment.svg'
defineProps<{
  title: string
  content: string
  date: string
}>()
</script>

<template>
  <div class="notice">
    <img :src="CommentIcon" class="notice-icon" alt="" />
    <div class="notice-body">
      <div class="notice-title">{{ title }}</div>
      <div class="notice-content">{{ content }}</div>
      <div class="notice-date">{{ date }}</div>
    </div>
  </div>
</template>

<style scoped>
.notice {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  gap: 0.5rem;
}

.notice-icon {
  flex-shrink: 0;
  width: 1.25rem;
  height: 1.25rem;
}

.notice-body {
  display: flex;
  flex-direction: column;
  flex: 1;
  gap: 0.15rem;
}

.notice-title {
  font-size: 1rem;
}

.notice-content {
  font-size: 0.8rem;
  color: var(--text-muted);
}

.notice-date {
  font-size: 0.7rem;
  color: var(--text-muted);
  text-align: right;
}
</style>
