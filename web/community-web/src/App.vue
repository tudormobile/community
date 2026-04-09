<script setup lang="ts">
import { RouterLink, RouterView, useRoute } from 'vue-router'
import { version as pkgVersion, name as appName } from '../package.json'
import type { AppConfig } from '@/types/config'
import appConfig from '@/assets/config.json'
import homeIcon from '@/assets/home.svg'
import mapIcon from '@/assets/map.svg'
import infoIcon from '@/assets/info.svg'
import calendarIcon from '@/assets/calendar_month.svg'

const version = import.meta.env.VITE_APP_VERSION || pkgVersion
const route = useRoute()
</script>

<template>
  <div class="top-bar">
    <div class="top-bar-text">
      <span class="top-bar-name">Community {{ (appConfig as AppConfig).name }}</span>
      <span class="top-bar-tagline">{{ (appConfig as AppConfig).tagline }}</span>
    </div>
    <img v-if="(appConfig as AppConfig).logoUrl" :src="(appConfig as AppConfig).logoUrl" class="top-bar-logo" alt="" />
  </div>

  <main-content :class="{ 'is-fixed': route.name === 'items' }">
    <RouterView />
  </main-content>

  <div class="bottom-bar">
    <nav class="tab-nav">
      <RouterLink to="/" class="tab-item">
        <img :src="homeIcon" class="tab-icon" alt="" />
        <span class="tab-label">Home</span>
      </RouterLink>
      <RouterLink to="/events" class="tab-item">
        <img :src="calendarIcon" class="tab-icon" alt="" />
        <span class="tab-label">Events</span>
      </RouterLink>
      <RouterLink to="/items" class="tab-item">
        <img :src="mapIcon" class="tab-icon" alt="" />
        <span class="tab-label">Items</span>
      </RouterLink>
      <RouterLink to="/about" class="tab-item">
        <img :src="infoIcon" class="tab-icon" alt="" />
        <span class="tab-label">About</span>
      </RouterLink>
    </nav>
    <footer class="app-footer">
      <span>{{ appName }}</span>
      <span>Copyright &copy; 2026 Bill Tudor</span>
      <span>v{{ version }}</span>
    </footer>
  </div>
</template>

<style scoped>
.top-bar {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 100;
  background: var(--brand);
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  padding: calc(0.6rem + env(safe-area-inset-top)) 0.5rem 0.3rem 1rem;
  box-shadow: 0 2px 8px rgba(8, 18, 38, 0.18);
  gap: 0.5rem;
}

.top-bar-text {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  min-width: 0;
  overflow: hidden;
}

.top-bar-name {
  font-size: 1.15rem;
  font-weight: 700;
  color: #ffffff;
  line-height: 1.2;
  letter-spacing: 0.01em;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.top-bar-tagline {
  font-size: 0.72rem;
  color: rgba(255, 255, 255, 0.8);
  font-weight: 400;
  line-height: 1.3;
  margin-top: 0.1rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.top-bar-logo {
  height: 2.4rem;
  width: auto;
  flex-shrink: 0;
  object-fit: contain;
}

main-content {
  display: block;
  padding-top: calc(var(--top-bar-height) + env(safe-area-inset-top));
  padding-bottom: 0.5rem;
}

main-content.is-fixed {
  position: fixed;
  top: calc(var(--top-bar-height) + env(safe-area-inset-top));
  left: 0;
  right: 0;
  bottom: 4.5rem;
  overflow: hidden;
  padding-bottom: 0;
}

.bottom-bar {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  z-index: 100;
  background: var(--surface);
  border-top: 1px solid var(--line);
  box-shadow: 0 -2px 12px rgba(8, 18, 38, 0.08);
}

.tab-nav {
  display: flex;
  justify-content: space-around;
  align-items: stretch;
  padding: 0.35rem 0.5rem 0;
}

.tab-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.15rem;
  padding: 0.45rem 0.25rem 0.55rem;
  text-decoration: none;
  color: var(--text-muted);
  font-size: 0.7rem;
  font-weight: 500;
  border-radius: var(--radius-sm);
  position: relative;
  transition: color 0.2s ease;
}

.tab-item::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 10%;
  right: 10%;
  height: 3px;
  background: var(--brand-strong);
  opacity: 0;
  transform: scaleX(0.4);
  transition: opacity 0.25s ease, transform 0.25s ease;
}

.tab-item.router-link-active {
  color: var(--brand-strong);
}

.tab-item.router-link-active::after {
  opacity: 1;
  transform: scaleX(1);
}

.tab-icon {
  width: 1.5rem;
  height: 1.5rem;
  opacity: 0.45;
  transition: transform 0.2s ease, opacity 0.2s ease, filter 0.2s ease;
  filter: none;
}

.tab-item.router-link-active .tab-icon {
  transform: translateY(-1px);
  opacity: 1;
  filter: invert(38%) sepia(72%) saturate(500%) hue-rotate(140deg) brightness(90%);
}

.app-footer {
  display: flex;
  justify-content: space-between;
  font-size: 0.6rem;
  color: var(--text-muted);
  padding: 0.2rem 0.75rem calc(0.2rem + env(safe-area-inset-bottom));
  opacity: 0.6;
}
</style>
