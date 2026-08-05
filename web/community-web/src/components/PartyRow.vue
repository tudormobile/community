<script setup lang="ts">
import { computed, ref } from 'vue'
import CameraCapture from '@/components/CameraCapture.vue'
import cameraIconSrc from '../assets/icons/camera.svg'
import type { EntityId } from '@/types/users'

const props = defineProps<{
  groupId: EntityId
  partyId: EntityId
  name: string
  presentCount: number
  totalCount: number
  status: 'all' | 'some' | 'none'
  expanded: boolean
  expandIconSrc: string
  accentColor: string
  thumbnailSrc?: string | null
}>()

const emit = defineEmits<{
  (e: 'tap-party'): void
  (e: 'toggle-expanded'): void
  (e: 'thumbnail-saved'): void
  (e: 'thumbnail-removed'): void
}>()

const spinTurns = ref(0)
const isCaptureOpen = ref(false)

const iconStyle = computed(() => ({
  transform: `rotate(${spinTurns.value * 360}deg)`,
}))

const expandedButtonStyle = computed(() => {
  if (!props.expanded) {
    return undefined
  }

  return {
    borderColor: props.accentColor,
    backgroundColor: `color-mix(in srgb, ${props.accentColor} 14%, white)`,
  }
})

function onTapParty() {
  emit('tap-party')
}

function onToggleExpanded() {
  spinTurns.value += 1
  emit('toggle-expanded')
}

function onTapCaptureThumbnail() {
  isCaptureOpen.value = true
}

function onCloseCapture() {
  isCaptureOpen.value = false
}

function onThumbnailSaved() {
  emit('thumbnail-saved')
}

function onThumbnailRemoved() {
  emit('thumbnail-removed')
}
</script>

<template>
  <article class="party-card">
    <div class="party-row">
      <button type="button" class="party-main" @click="onTapParty">
        <span class="status-dot" :class="status" aria-hidden="true"></span>
        <span class="party-name">{{ name }}</span>
        <span class="party-count">{{ presentCount }} / {{ totalCount }}</span>
      </button>

      <button
        type="button"
        class="expand-button"
        :style="expandedButtonStyle"
        :aria-label="expanded ? 'Hide members' : 'Show members'"
        @click.stop="onToggleExpanded"
      >
        <img :src="expandIconSrc" alt="" class="expand-icon" :style="iconStyle" />
      </button>
    </div>

    <div v-if="expanded" class="members-panel">
      <button
        type="button"
        class="capture-thumbnail-button"
        :class="{ 'has-thumbnail': Boolean(thumbnailSrc) }"
        :style="expandedButtonStyle"
        aria-label="Capture party image"
        @click.stop="onTapCaptureThumbnail"
      >
        <img
          v-if="thumbnailSrc"
          :src="thumbnailSrc"
          alt="Party thumbnail"
          class="capture-thumbnail-image"
        />
        <img v-else :src="cameraIconSrc" alt="" class="capture-thumbnail-icon" />
      </button>

      <CameraCapture
        v-if="isCaptureOpen"
        :group-id="String(groupId)"
        :party-id="String(partyId)"
        :has-thumbnail="Boolean(thumbnailSrc)"
        @saved="onThumbnailSaved"
        @removed="onThumbnailRemoved"
        @close="onCloseCapture"
      />

      <slot name="members"></slot>
    </div>
  </article>
</template>

<style scoped>
.party-card {
  border: 1px solid var(--line);
  border-radius: var(--radius-md);
  background: var(--surface-soft);
}

.party-row {
  display: flex;
  align-items: stretch;
  gap: 0.4rem;
  padding: 0.25rem;
}

.party-main {
  min-height: 2.7rem;
  border: 1px solid transparent;
  background: transparent;
  border-radius: calc(var(--radius-md) - 0.2rem);
  display: flex;
  align-items: center;
  gap: 0.55rem;
  flex: 1;
  text-align: left;
  padding: 0.55rem 0.65rem;
}

.party-name {
  flex: 1;
  font-weight: 600;
  color: var(--text-strong);
}

.party-count {
  color: var(--text-muted);
  font-weight: 600;
}

.status-dot {
  width: 0.62rem;
  height: 0.62rem;
  border-radius: 999px;
  display: inline-block;
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

.expand-button {
  min-height: 2.7rem;
  min-width: 2.7rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 0.4rem;
  border-radius: calc(var(--radius-md) - 0.2rem);
  transition: background-color 0.2s ease, border-color 0.2s ease;
}

.expand-icon {
  width: 1.2rem;
  height: 1.2rem;
  transition: transform 0.32s linear;
}

.members-panel {
  border-top: 1px solid var(--line);
  padding: 0.35rem 0.5rem 0.5rem;
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
}

.capture-thumbnail-button {
  width: 200px;
  height: 200px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  padding: 0;
  border-radius: calc(var(--radius-md) - 0.2rem);
  transition: background-color 0.2s ease, border-color 0.2s ease;
}

.capture-thumbnail-button.has-thumbnail {
  border-color: color-mix(in srgb, var(--brand) 30%, var(--line));
}

.capture-thumbnail-icon {
  width: 5.25rem;
  height: 5.25rem;
}

.capture-thumbnail-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
</style>