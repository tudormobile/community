<script setup lang="ts">
import { RouterLink, RouterView, useRoute } from 'vue-router'
import { version as pkgVersion } from '../package.json'
import homeIcon from '@/assets/home.svg'
import mapIcon from '@/assets/map.svg'
import infoIcon from '@/assets/info.svg'

const version = import.meta.env.VITE_APP_VERSION || pkgVersion
const route = useRoute()
</script>

<template>
  <main-content :class="{ 'is-fixed': route.name === 'items' }">
    <RouterView />
  </main-content>

  <div class="bottom-bar">
    <nav class="tab-nav">
      <RouterLink to="/" class="tab-item">
        <img :src="homeIcon" class="tab-icon" alt="" />
        <span class="tab-label">Home</span>
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
      <span>Copyright &copy; 2026 Bill Tudor</span>
      <span>v{{ version }}</span>
    </footer>
  </div>
</template>

<style scoped>
main-content {
  display: block;
  padding-bottom: 5.5rem;
}

main-content.is-fixed {
  position: fixed;
  top: 0;
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
  left: 20%;
  right: 20%;
  height: 3px;
  background: var(--brand);
  border-radius: 3px 3px 0 0;
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
