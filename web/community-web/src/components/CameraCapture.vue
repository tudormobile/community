<script setup lang="ts">
import { computed, nextTick, onBeforeUnmount, ref } from 'vue'
import { removeThumbnail, saveThumbnail, type ThumbnailShape } from '@/lib/thumbnailStore'

const props = defineProps<{
  partyId: string
  hasThumbnail: boolean
}>()

const emit = defineEmits<{
  (e: 'saved'): void
  (e: 'removed'): void
  (e: 'close'): void
}>()

const TARGET_SIZE = 400

const videoElement = ref<HTMLVideoElement | null>(null)
const stream = ref<MediaStream | null>(null)
const isStreaming = ref(false)
const isSaving = ref(false)
const isRemoving = ref(false)
const errorMessage = ref('')
const shape = ref<ThumbnailShape>('square')

const shapeLabel = computed(() => (shape.value === 'circle' ? 'Circular' : 'Square'))

const stopStream = () => {
  if (!stream.value) {
    return
  }

  for (const track of stream.value.getTracks()) {
    track.stop()
  }

  stream.value = null
  isStreaming.value = false

  if (videoElement.value) {
    videoElement.value.srcObject = null
  }
}

const startCamera = async () => {
  errorMessage.value = ''

  try {
    const preferredVideoConstraints: MediaTrackConstraints[] = [
      {
        facingMode: { ideal: 'environment' },
        width: { ideal: 1920 },
        height: { ideal: 1080 },
      },
      {
        facingMode: { ideal: 'user' },
        width: { ideal: 1920 },
        height: { ideal: 1080 },
      },
      {
        width: { ideal: 1920 },
        height: { ideal: 1080 },
      },
    ]

    let media: MediaStream | null = null
    for (const video of preferredVideoConstraints) {
      try {
        media = await navigator.mediaDevices.getUserMedia({ audio: false, video })
        break
      } catch {
        // Try the next camera preference.
      }
    }

    if (!media) {
      media = await navigator.mediaDevices.getUserMedia({ video: true, audio: false })
    }

    stream.value = media
    isStreaming.value = true

    await nextTick()

    if (videoElement.value) {
      videoElement.value.srcObject = media
      await videoElement.value.play()
    }
  } catch (error) {
    errorMessage.value =
      error instanceof Error
        ? error.message
        : 'Camera permission denied or no camera available on this device.'
  }
}

const toggleShape = () => {
  shape.value = shape.value === 'circle' ? 'square' : 'circle'
}

const captureBlob = async (): Promise<Blob> => {
  const video = videoElement.value
  if (!video) {
    throw new Error('Video element not ready.')
  }

  const vw = video.videoWidth
  const vh = video.videoHeight
  if (!vw || !vh) {
    throw new Error('Camera feed not ready yet. Try again in a second.')
  }

  const rect = video.getBoundingClientRect()
  const containerWidth = rect.width
  const containerHeight = rect.height
  const overlaySize = Math.min(containerWidth, containerHeight) * 0.7

  // Map visible viewport coords to source coords when using object-fit: cover.
  const scale = Math.max(containerWidth / vw, containerHeight / vh)
  const displayedWidth = vw * scale
  const displayedHeight = vh * scale
  const cropOffsetX = (displayedWidth - containerWidth) / 2
  const cropOffsetY = (displayedHeight - containerHeight) / 2

  const overlayLeft = (containerWidth - overlaySize) / 2
  const overlayTop = (containerHeight - overlaySize) / 2

  const sourceX = (overlayLeft + cropOffsetX) / scale
  const sourceY = (overlayTop + cropOffsetY) / scale
  const sourceSize = overlaySize / scale

  const canvas = document.createElement('canvas')
  canvas.width = TARGET_SIZE
  canvas.height = TARGET_SIZE
  const context = canvas.getContext('2d')

  if (!context) {
    throw new Error('Could not access canvas context.')
  }

  context.clearRect(0, 0, TARGET_SIZE, TARGET_SIZE)

  if (shape.value === 'circle') {
    context.beginPath()
    context.arc(TARGET_SIZE / 2, TARGET_SIZE / 2, TARGET_SIZE / 2, 0, Math.PI * 2)
    context.closePath()
    context.clip()
  }

  context.drawImage(video, sourceX, sourceY, sourceSize, sourceSize, 0, 0, TARGET_SIZE, TARGET_SIZE)

  return await new Promise((resolve, reject) => {
    canvas.toBlob(
      (blob) => {
        if (!blob) {
          reject(new Error('Failed to create image blob.'))
          return
        }

        resolve(blob)
      },
      'image/png',
      0.95,
    )
  })
}

const captureAndSave = async () => {
  isSaving.value = true
  errorMessage.value = ''

  try {
    const blob = await captureBlob()
    await saveThumbnail(props.partyId, blob, shape.value)
    stopStream()
    emit('saved')
    emit('close')
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Unable to capture thumbnail.'
  } finally {
    isSaving.value = false
  }
}

const removeCurrentThumbnail = async () => {
  isRemoving.value = true
  errorMessage.value = ''

  try {
    await removeThumbnail(props.partyId)
    stopStream()
    emit('removed')
    emit('close')
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : 'Unable to remove thumbnail.'
  } finally {
    isRemoving.value = false
  }
}

const closeCapture = () => {
  stopStream()
  emit('close')
}

onBeforeUnmount(() => {
  stopStream()
})
</script>

<template>
  <section class="capture-shell" aria-label="Thumbnail capture">
    <div class="controls-row">
      <button v-if="!isStreaming" type="button" @click="startCamera">Start Camera</button>
      <template v-else>
        <button type="button" :disabled="isSaving || isRemoving" @click="captureAndSave">
          Capture {{ shapeLabel }}
        </button>
        <button type="button" :disabled="isSaving || isRemoving" @click="toggleShape">
          Use {{ shape === 'circle' ? 'Square' : 'Circle' }}
        </button>
        <button type="button" :disabled="isSaving || isRemoving" @click="stopStream">Stop Camera</button>
      </template>

      <button
        v-if="hasThumbnail"
        type="button"
        class="remove-button"
        :disabled="isSaving || isRemoving"
        @click="removeCurrentThumbnail"
      >
        Remove
      </button>

      <button type="button" :disabled="isSaving || isRemoving" @click="closeCapture">Done</button>
    </div>

    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

    <div v-if="isStreaming" class="video-stage">
      <video ref="videoElement" autoplay playsinline muted></video>
      <div class="overlay" :class="shape" aria-hidden="true"></div>
    </div>
  </section>
</template>

<style scoped>
.capture-shell {
  border: 1px solid var(--line);
  border-radius: var(--radius-md);
  background: var(--surface-soft);
  padding: 0.65rem;
}

.controls-row {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 0.65rem;
}

.error-message {
  color: var(--text-error);
  margin-bottom: 0.5rem;
}

.video-stage {
  position: relative;
  width: min(100%, 560px);
  aspect-ratio: 4 / 3;
  background: #111;
  border-radius: var(--radius-sm);
  overflow: hidden;
}

video {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.overlay {
  position: absolute;
  left: 50%;
  top: 50%;
  width: 70%;
  aspect-ratio: 1 / 1;
  transform: translate(-50%, -50%);
  border: 2px solid #ffffff;
  box-shadow: 0 0 0 9999px rgb(0 0 0 / 0.35);
  pointer-events: none;
}

.overlay.circle {
  border-radius: 50%;
}

.remove-button {
  border-color: color-mix(in srgb, var(--danger) 45%, var(--line));
  background: color-mix(in srgb, var(--danger) 10%, white);
  color: var(--danger);
}
</style>
