<script setup lang="ts">
import { version as pkgVersion, name as appName, copyright as AppCopyright, license as AppLicense } from '../../package.json'
import logoIcon from '@/assets/icons/logo.svg'
import AboutService from './AboutService.vue';
import AboutData from './AboutData.vue';
const version = import.meta.env.VITE_APP_VERSION || pkgVersion

</script>

<template>
    <div class="about">
        <AboutService />
        <AboutData />
        <div style="display: flex; flex-direction: row; align-items: center;">
            <span><img :src="logoIcon" class="logo" alt="" /></span>
            <div style="display: flex; flex-direction: column; gap: 0.0rem; margin-left: 0.5rem;">
            <div class="title">{{ appName }} v{{ pkgVersion }}</div>
            <div class="subtitle">{{ AppCopyright }}</div>
            <div class="subtitle">License: {{ AppLicense }}</div>
        </div>
    </div>   
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
.logo {
    width: 3rem;
    height: 3rem;
}

</style>