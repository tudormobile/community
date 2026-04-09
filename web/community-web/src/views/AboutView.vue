<script setup lang="ts">
import AboutCommunity from '@/components/AboutCommunity.vue';
import AboutApp from '@/components/AboutApp.vue';
</script>
<template>
  <div class="about">
    <AboutApp />
    <AboutCommunity />
  </div>
</template>

<style>
.about {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  align-items: center;
}
</style>
