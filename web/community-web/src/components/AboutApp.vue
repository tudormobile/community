<script setup lang="ts">
import type { AppConfig } from '@/types/config'
import appConfig from '@/assets/config.json'

const config = appConfig as AppConfig
</script>

<template>
    <div class="about">
        <div style="display: flex; flex-direction: row; align-items: center;">
            <span><img v-if="config.logoUrl" :src="config.logoUrl" class="logo" alt="" /></span>
            <div style="display: flex; flex-direction: column; gap: 0.0rem; margin-left: 0.5rem;">
                <span class="title">{{ config.name }}</span>
                <div class="subtitle">{{ config.tagline }}</div>
            </div>
        </div>
        <div class="body">{{ config.description }}</div>
        <dl>
            <dt>Item Name</dt>
            <dd>{{config.name}} - {{ config.itemName }}</dd>
            <dt>Location</dt>
            <dd>{{ config.homeLocation.adr}} ({{ config.homeLocation.lat.toFixed(3) }},{{ config.homeLocation.lng.toFixed(3) }})</dd>
        </dl>
    </div>
</template>
<style scoped>
.about {
    display: flex;
    flex-direction: column;
    gap: 0.0rem;
    align-items: flex-start;
}

.title {
    font-size: 1rem;
    font-weight: bold;
}

.subtitle {
    font-size: 0.75rem;
    color: var(--text-muted);
}

.body {
    padding: 0.5rem 0;
    font-size: 0.875rem;
    color: var(--text);
}

.logo {
    width: 4rem;
    height: 4rem;
}
dl {
    margin: 0;
    font-size: 0.75rem;
}
</style>